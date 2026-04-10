using System;
using System.Collections.Generic;
using System.Threading;
using A9N.Agent.Gateway;
using A9N.Agent.Gateway.Platforms;
using A9NDesktop.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.ApplicationModel.Resources;

namespace A9NDesktop.Views;

public sealed partial class IntegrationsPage : Page
{
    private static readonly ResourceLoader ResourceLoader = new();
    private bool _suppressWhatsAppToggle;
    private bool _suppressWebhookToggle;

    public IntegrationsPage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        RefreshNativeGatewayStatus();
        RefreshGatewayStatus();
        RefreshTelegramDisplay();
        RefreshDiscordDisplay();
        RefreshSlackDisplay();
        RefreshWhatsAppDisplay();
        RefreshMatrixDisplay();
        RefreshWebhookDisplay();
    }

    // =========================================================================
    // Native Gateway (C#) Status
    // =========================================================================

    private void RefreshNativeGatewayStatus()
    {
        bool running = A9NEnvironment.IsNativeGatewayRunning();

        NativeGatewayStatusText.Text = running ? "Running" : "Stopped";
        NativeGatewayIndicator.Fill = running
            ? (Brush)Application.Current.Resources["ConnectionOnlineBrush"]
            : (Brush)Application.Current.Resources["ConnectionOfflineBrush"];

        var adapterStatus = A9NEnvironment.GetNativeAdapterStatus();
        if (running && adapterStatus.Count > 0)
        {
            var parts = new List<string>();
            foreach (var (platform, connected) in adapterStatus)
                parts.Add($"{platform}: {(connected ? "Connected" : "Disconnected")}");
            NativeGatewayStateText.Text = string.Join(" | ", parts);
        }
        else
        {
            var tgToken = A9NEnvironment.ReadPlatformSetting("telegram", "token");
            var dcToken = A9NEnvironment.ReadPlatformSetting("discord", "token");
            bool hasNativeTokens = !string.IsNullOrWhiteSpace(tgToken) || !string.IsNullOrWhiteSpace(dcToken);

            NativeGatewayStateText.Text = hasNativeTokens
                ? "Native gateway is not running. Click Start to launch it."
                : "No Telegram or Discord tokens configured. Save a token below to get started.";
        }

        NativeGatewayToggleButton.Content = running ? "Stop Native Gateway" : "Start Native Gateway";

        // Build per-adapter status indicators
        AdapterStatusPanel.Children.Clear();
        foreach (var platform in new[] { "Telegram", "Discord" })
        {
            var indicator = new Microsoft.UI.Xaml.Shapes.Ellipse { Width = 8, Height = 8, VerticalAlignment = VerticalAlignment.Center };
            if (adapterStatus.TryGetValue(platform, out var connected) && connected)
                indicator.Fill = (Brush)Application.Current.Resources["ConnectionOnlineBrush"];
            else
                indicator.Fill = (Brush)Application.Current.Resources["ConnectionOfflineBrush"];

            var label = new TextBlock
            {
                Text = platform,
                FontSize = 12,
                Foreground = (Brush)Application.Current.Resources["AppTextSecondaryBrush"],
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(4, 0, 12, 0)
            };

            AdapterStatusPanel.Children.Add(indicator);
            AdapterStatusPanel.Children.Add(label);
        }
    }

    private async void NativeGatewayToggle_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var gateway = App.Services.GetRequiredService<GatewayService>();

            if (gateway.IsRunning)
            {
                await gateway.StopAsync();
            }
            else
            {
                StartNativeGatewayFromUI(gateway);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Native gateway toggle error: {ex.Message}");
        }

        DispatcherQueue.TryEnqueue(async () =>
        {
            await System.Threading.Tasks.Task.Delay(1500);
            RefreshNativeGatewayStatus();
        });
    }

    private async void RestartNativeGateway_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var gateway = App.Services.GetRequiredService<GatewayService>();

            if (gateway.IsRunning)
                await gateway.StopAsync();

            // Brief pause to let connections close
            await System.Threading.Tasks.Task.Delay(500);

            StartNativeGatewayFromUI(gateway);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Native gateway restart error: {ex.Message}");
        }

        DispatcherQueue.TryEnqueue(async () =>
        {
            await System.Threading.Tasks.Task.Delay(2000);
            RefreshNativeGatewayStatus();
        });
    }

    private static void StartNativeGatewayFromUI(GatewayService gateway)
    {
        // Wire agent handler if not already set
        var agent = App.Services.GetRequiredService<A9N.Agent.Core.Agent>();
        gateway.SetAgentHandler(async (sessionId, userMessage, platform) =>
        {
            var session = new A9N.Agent.Core.Session { Id = sessionId, Platform = platform };
            return await agent.ChatAsync(userMessage, session, CancellationToken.None);
        });

        var adapters = new List<IPlatformAdapter>();

        var tgToken = A9NEnvironment.ReadPlatformSetting("telegram", "token");
        if (!string.IsNullOrWhiteSpace(tgToken))
            adapters.Add(new TelegramAdapter(tgToken));

        var dcToken = A9NEnvironment.ReadPlatformSetting("discord", "token");
        if (!string.IsNullOrWhiteSpace(dcToken))
            adapters.Add(new DiscordAdapter(dcToken));

        if (adapters.Count > 0)
        {
            _ = System.Threading.Tasks.Task.Run(async () =>
            {
                try { await gateway.StartAsync(adapters, CancellationToken.None); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Gateway start failed: {ex.Message}"); }
            });
        }
    }

    // =========================================================================
    // Python Gateway Status (advanced platforms)
    // =========================================================================

    private void RefreshGatewayStatus()
    {
        bool running = A9NEnvironment.IsGatewayRunning();
        bool installed = A9NEnvironment.A9NInstalled;

        GatewayStatusText.Text = running ? "Running" : "Stopped";
        GatewayIndicator.Fill = running
            ? (Brush)Application.Current.Resources["ConnectionOnlineBrush"]
            : (Brush)Application.Current.Resources["ConnectionOfflineBrush"];

        string state = A9NEnvironment.ReadGatewayState();
        GatewayStateText.Text = running
            ? $"State: {state}"
            : installed ? "Gateway is not running. Click Start to launch it." : "A9N CLI not found. Install A9N first.";

        GatewayToggleButton.Content = running ? "Stop Gateway" : "Start Gateway";
        GatewayToggleButton.IsEnabled = installed;
    }

    private void GatewayToggle_Click(object sender, RoutedEventArgs e)
    {
        if (A9NEnvironment.IsGatewayRunning())
        {
            A9NEnvironment.StopGateway();
        }
        else
        {
            A9NEnvironment.StartGateway();
        }

        // Small delay then refresh
        DispatcherQueue.TryEnqueue(async () =>
        {
            await System.Threading.Tasks.Task.Delay(1500);
            RefreshGatewayStatus();
        });
    }

    private void GatewayRefresh_Click(object sender, RoutedEventArgs e)
    {
        RefreshAll();
    }

    // =========================================================================
    // Telegram
    // =========================================================================

    private void RefreshTelegramDisplay()
    {
        var configToken = A9NEnvironment.ReadPlatformSetting("telegram", "token");
        var legacyToken = A9NEnvironment.ReadIntegrationSetting("telegram_bot_token");
        var envConfigured = A9NEnvironment.TelegramConfigured;
        var token = configToken ?? legacyToken;
        var hasToken = !string.IsNullOrWhiteSpace(token) || envConfigured;

        TelegramStatusText.Text = hasToken
            ? ResourceLoader.GetString("StatusConfigured")
            : ResourceLoader.GetString("StatusNotDetected");

        TelegramMaskedText.Text = !string.IsNullOrWhiteSpace(token)
            ? MaskToken(token)
            : envConfigured ? "Set via environment variable" : "Not configured";
    }

    private async void SaveTelegram_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var token = TelegramTokenBox.Password.Trim();
            if (string.IsNullOrEmpty(token))
            {
                SetStatus(TelegramSaveStatus, "Token cannot be empty.", false);
                return;
            }

            await A9NEnvironment.SavePlatformSettingAsync("telegram", "token", token);
            await A9NEnvironment.SavePlatformSettingAsync("telegram", "enabled", "true");
            SetStatus(TelegramSaveStatus, "Saved to config.yaml.", true);
            TelegramTokenBox.Password = "";
            RefreshTelegramDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(TelegramSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    // =========================================================================
    // Discord
    // =========================================================================

    private void RefreshDiscordDisplay()
    {
        var configToken = A9NEnvironment.ReadPlatformSetting("discord", "token");
        var legacyToken = A9NEnvironment.ReadIntegrationSetting("discord_bot_token");
        var envConfigured = A9NEnvironment.DiscordConfigured;
        var token = configToken ?? legacyToken;
        var hasToken = !string.IsNullOrWhiteSpace(token) || envConfigured;

        DiscordStatusText.Text = hasToken
            ? ResourceLoader.GetString("StatusConfigured")
            : ResourceLoader.GetString("StatusNotDetected");

        DiscordMaskedText.Text = !string.IsNullOrWhiteSpace(token)
            ? MaskToken(token)
            : envConfigured ? "Set via environment variable" : "Not configured";
    }

    private async void SaveDiscord_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var token = DiscordTokenBox.Password.Trim();
            if (string.IsNullOrEmpty(token))
            {
                SetStatus(DiscordSaveStatus, "Token cannot be empty.", false);
                return;
            }

            await A9NEnvironment.SavePlatformSettingAsync("discord", "token", token);
            await A9NEnvironment.SavePlatformSettingAsync("discord", "enabled", "true");
            SetStatus(DiscordSaveStatus, "Saved to config.yaml.", true);
            DiscordTokenBox.Password = "";
            RefreshDiscordDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(DiscordSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    // =========================================================================
    // Slack
    // =========================================================================

    private void RefreshSlackDisplay()
    {
        var configToken = A9NEnvironment.ReadPlatformSetting("slack", "token");
        var envConfigured = A9NEnvironment.SlackConfigured;
        var hasToken = !string.IsNullOrWhiteSpace(configToken) || envConfigured;

        SlackStatusText.Text = hasToken
            ? ResourceLoader.GetString("StatusConfigured")
            : ResourceLoader.GetString("StatusNotDetected");

        SlackMaskedText.Text = !string.IsNullOrWhiteSpace(configToken)
            ? MaskToken(configToken)
            : envConfigured ? "Set via environment variable" : "Not configured";
    }

    private async void SaveSlack_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var botToken = SlackBotTokenBox.Password.Trim();
            var appToken = SlackAppTokenBox.Password.Trim();

            if (string.IsNullOrEmpty(botToken))
            {
                SetStatus(SlackSaveStatus, "Bot token cannot be empty.", false);
                return;
            }

            await A9NEnvironment.SavePlatformSettingAsync("slack", "token", botToken);
            await A9NEnvironment.SavePlatformSettingAsync("slack", "enabled", "true");

            if (!string.IsNullOrEmpty(appToken))
            {
                await A9NEnvironment.SavePlatformSettingAsync("slack", "app_token", appToken);
            }

            SetStatus(SlackSaveStatus, "Saved to config.yaml.", true);
            SlackBotTokenBox.Password = "";
            SlackAppTokenBox.Password = "";
            RefreshSlackDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(SlackSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    // =========================================================================
    // WhatsApp
    // =========================================================================

    private void RefreshWhatsAppDisplay()
    {
        var envConfigured = A9NEnvironment.WhatsAppConfigured;

        WhatsAppStatusText.Text = envConfigured
            ? ResourceLoader.GetString("StatusConfigured")
            : ResourceLoader.GetString("StatusNotDetected");

        WhatsAppMaskedText.Text = envConfigured ? "Enabled" : "Not enabled";

        _suppressWhatsAppToggle = true;
        WhatsAppEnabledToggle.IsOn = envConfigured;
        _suppressWhatsAppToggle = false;
    }

    private async void WhatsAppToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_suppressWhatsAppToggle) return;

        try
        {
            string value = WhatsAppEnabledToggle.IsOn ? "true" : "false";
            await A9NEnvironment.SavePlatformSettingAsync("whatsapp", "enabled", value);
            SetStatus(WhatsAppSaveStatus, "Saved to config.yaml.", true);
            RefreshWhatsAppDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(WhatsAppSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    // =========================================================================
    // Matrix
    // =========================================================================

    private void RefreshMatrixDisplay()
    {
        var configToken = A9NEnvironment.ReadPlatformSetting("matrix", "token");
        var envConfigured = A9NEnvironment.MatrixConfigured;
        var hasToken = !string.IsNullOrWhiteSpace(configToken) || envConfigured;

        MatrixStatusText.Text = hasToken
            ? ResourceLoader.GetString("StatusConfigured")
            : ResourceLoader.GetString("StatusNotDetected");

        MatrixMaskedText.Text = !string.IsNullOrWhiteSpace(configToken)
            ? MaskToken(configToken)
            : envConfigured ? "Set via environment variable" : "Not configured";

        // Load homeserver into text box if present in config
        var homeserver = A9NEnvironment.ReadPlatformSetting("matrix", "homeserver");
        if (!string.IsNullOrWhiteSpace(homeserver) && string.IsNullOrEmpty(MatrixHomeserverBox.Text))
        {
            MatrixHomeserverBox.Text = homeserver;
        }
    }

    private async void SaveMatrix_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var token = MatrixTokenBox.Password.Trim();
            var homeserver = MatrixHomeserverBox.Text.Trim();

            if (string.IsNullOrEmpty(token))
            {
                SetStatus(MatrixSaveStatus, "Access token cannot be empty.", false);
                return;
            }

            await A9NEnvironment.SavePlatformSettingAsync("matrix", "token", token);
            await A9NEnvironment.SavePlatformSettingAsync("matrix", "enabled", "true");

            if (!string.IsNullOrEmpty(homeserver))
            {
                await A9NEnvironment.SavePlatformSettingAsync("matrix", "homeserver", homeserver);
            }

            SetStatus(MatrixSaveStatus, "Saved to config.yaml.", true);
            MatrixTokenBox.Password = "";
            RefreshMatrixDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(MatrixSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    // =========================================================================
    // Webhook
    // =========================================================================

    private void RefreshWebhookDisplay()
    {
        var envConfigured = A9NEnvironment.WebhookConfigured;

        WebhookStatusText.Text = envConfigured
            ? ResourceLoader.GetString("StatusConfigured")
            : ResourceLoader.GetString("StatusNotDetected");

        WebhookMaskedText.Text = envConfigured ? "Enabled" : "Not enabled";

        _suppressWebhookToggle = true;
        WebhookEnabledToggle.IsOn = envConfigured;
        _suppressWebhookToggle = false;

        // Load port from config
        var port = A9NEnvironment.ReadPlatformSetting("webhook", "port");
        if (!string.IsNullOrWhiteSpace(port) && string.IsNullOrEmpty(WebhookPortBox.Text))
        {
            WebhookPortBox.Text = port;
        }
    }

    private async void WebhookToggle_Toggled(object sender, RoutedEventArgs e)
    {
        if (_suppressWebhookToggle) return;

        try
        {
            string value = WebhookEnabledToggle.IsOn ? "true" : "false";
            await A9NEnvironment.SavePlatformSettingAsync("webhook", "enabled", value);
            RefreshWebhookDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(WebhookSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    private async void SaveWebhook_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await A9NEnvironment.SavePlatformSettingAsync("webhook", "enabled", "true");

            var port = WebhookPortBox.Text.Trim();
            if (!string.IsNullOrEmpty(port))
            {
                await A9NEnvironment.SavePlatformSettingAsync("webhook", "port", port);
            }

            var secret = WebhookSecretBox.Password.Trim();
            if (!string.IsNullOrEmpty(secret))
            {
                await A9NEnvironment.SavePlatformSettingAsync("webhook", "secret", secret);
            }

            SetStatus(WebhookSaveStatus, "Saved to config.yaml.", true);
            WebhookSecretBox.Password = "";
            RefreshWebhookDisplay();
        }
        catch (Exception ex)
        {
            SetStatus(WebhookSaveStatus, $"Error: {ex.Message}", false);
        }
    }

    // =========================================================================
    // Shared helpers
    // =========================================================================

    private void OpenLogs_Click(object sender, RoutedEventArgs e)
    {
        A9NEnvironment.OpenLogs();
    }

    private void OpenConfig_Click(object sender, RoutedEventArgs e)
    {
        A9NEnvironment.OpenConfig();
    }

    private static string MaskToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return "";
        if (token.Length <= 4) return "****";
        return "****" + token[^4..];
    }

    private void SetStatus(TextBlock statusBlock, string message, bool success)
    {
        statusBlock.Text = message;
        statusBlock.Foreground = success
            ? (Brush)Application.Current.Resources["ConnectionOnlineBrush"]
            : (Brush)Application.Current.Resources["ConnectionOfflineBrush"];
    }
}
