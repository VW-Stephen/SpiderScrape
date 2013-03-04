partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.web_browser = new System.Windows.Forms.WebBrowser();
        this.text_log = new System.Windows.Forms.TextBox();
        this.start_button = new System.Windows.Forms.Button();
        this.pause_button = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // web_browser
        // 
        this.web_browser.AllowWebBrowserDrop = false;
        this.web_browser.Location = new System.Drawing.Point(13, 13);
        this.web_browser.MinimumSize = new System.Drawing.Size(20, 20);
        this.web_browser.Name = "web_browser";
        this.web_browser.ScriptErrorsSuppressed = true;
        this.web_browser.Size = new System.Drawing.Size(477, 364);
        this.web_browser.TabIndex = 0;
        // 
        // text_log
        // 
        this.text_log.Enabled = false;
        this.text_log.Location = new System.Drawing.Point(515, 13);
        this.text_log.Multiline = true;
        this.text_log.Name = "text_log";
        this.text_log.ReadOnly = true;
        this.text_log.Size = new System.Drawing.Size(477, 321);
        this.text_log.TabIndex = 1;
        // 
        // start_button
        // 
        this.start_button.Location = new System.Drawing.Point(515, 354);
        this.start_button.Name = "start_button";
        this.start_button.Size = new System.Drawing.Size(230, 23);
        this.start_button.TabIndex = 2;
        this.start_button.Text = "Start Scraping";
        this.start_button.UseVisualStyleBackColor = true;
        this.start_button.Click += new System.EventHandler(this.start_button_click);
        // 
        // pause_button
        // 
        this.pause_button.Location = new System.Drawing.Point(762, 354);
        this.pause_button.Name = "pause_button";
        this.pause_button.Size = new System.Drawing.Size(230, 23);
        this.pause_button.TabIndex = 3;
        this.pause_button.Text = "Pause Scraping";
        this.pause_button.UseVisualStyleBackColor = true;
        this.pause_button.Click += new System.EventHandler(this.pause_button_click);
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1004, 394);
        this.Controls.Add(this.pause_button);
        this.Controls.Add(this.start_button);
        this.Controls.Add(this.text_log);
        this.Controls.Add(this.web_browser);
        this.Name = "MainForm";
        this.Text = "SpiderScrape";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.WebBrowser web_browser;
    private System.Windows.Forms.TextBox text_log;
    private System.Windows.Forms.Button start_button;
    private System.Windows.Forms.Button pause_button;

}
