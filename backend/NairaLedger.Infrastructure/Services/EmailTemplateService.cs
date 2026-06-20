namespace NairaLedger.Infrastructure.Services;

public interface IEmailTemplateService
{
    string BuildTemplate(string title, string body);
}

public class EmailTemplateService : IEmailTemplateService
{
    public string BuildTemplate(string title, string body)
    {
        return $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <title>{title}</title>
            <style>
              body {{
                font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                background-color: #f8fafc;
                margin: 0;
                padding: 0;
                -webkit-text-size-adjust: 100%;
                -ms-text-size-adjust: 100%;
              }}
              .container {{
                max-width: 600px;
                margin: 0 auto;
                padding: 20px;
              }}
              .header {{
                background-color: #0f172a;
                color: #ffffff;
                padding: 30px 20px;
                text-align: center;
                border-radius: 8px 8px 0 0;
              }}
              .header h1 {{
                margin: 0;
                font-size: 24px;
                font-weight: 700;
              }}
              .content {{
                background-color: #ffffff;
                padding: 30px 20px;
                border-left: 1px solid #e2e8f0;
                border-right: 1px solid #e2e8f0;
              }}
              .content p {{
                margin: 0 0 16px;
                line-height: 1.6;
                color: #334155;
              }}
              .button {{
                display: inline-block;
                background-color: #0f172a;
                color: #ffffff;
                text-decoration: none;
                padding: 12px 24px;
                border-radius: 6px;
                font-weight: 600;
                margin: 10px 0;
              }}
              .footer {{
                background-color: #f1f5f9;
                padding: 20px;
                text-align: center;
                border-radius: 0 0 8px 8px;
                border: 1px solid #e2e8f0;
                font-size: 12px;
                color: #64748b;
              }}
              @media (max-width: 600px) {{
                .container {{ padding: 10px; }}
                .header {{ padding: 20px 15px; }}
                .content {{ padding: 20px 15px; }}
              }}
            </style>
            </head>
            <body>
            <div class=""container"">
              <div class=""header"">
                <h1>NairaLedger</h1>
              </div>
              <div class=""content"">
                {body}
              </div>
              <div class=""footer"">
                <p>&copy; {DateTime.UtcNow.Year} NairaLedger. All rights reserved.</p>
                <p>This is an automated message, please do not reply.</p>
              </div>
            </div>
            </body>
            </html>";
    }
}