namespace PayVault.Domain.Templates
{
    public static class EmailTemplates
    {
        public static string ConfirmAccount(string name, string confirmationLink)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Confirm Your Account - PayVault</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
        }}
        .container {{
            max-width: 500px;
            width: 100%;
            background: white;
            border-radius: 20px;
            box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 100%);
            padding: 40px 30px;
            text-align: center;
        }}
        .logo {{
            font-size: 28px;
            font-weight: 700;
            color: white;
            margin-bottom: 8px;
            letter-spacing: 1px;
        }}
        .logo span {{
            color: #00d4aa;
        }}
        .tagline {{
            color: rgba(255,255,255,0.7);
            font-size: 14px;
        }}
        .content {{
            padding: 40px 30px;
        }}
        .greeting {{
            font-size: 20px;
            font-weight: 600;
            color: #1a1a2e;
            margin-bottom: 20px;
        }}
        .message {{
            color: #4a5568;
            line-height: 1.8;
            margin-bottom: 30px;
            font-size: 15px;
        }}
        .button {{
            display: inline-block;
            background: linear-gradient(135deg, #00d4aa 0%, #00b894 100%);
            color: white;
            text-decoration: none;
            padding: 16px 40px;
            border-radius: 50px;
            font-weight: 600;
            font-size: 16px;
            transition: transform 0.2s, box-shadow 0.2s;
            box-shadow: 0 10px 20px rgba(0, 212, 170, 0.3);
        }}
        .button:hover {{
            transform: translateY(-2px);
            box-shadow: 0 15px 30px rgba(0, 212, 170, 0.4);
        }}
        .footer {{
            background: #f7fafc;
            padding: 25px 30px;
            text-align: center;
            border-top: 1px solid #e2e8f0;
        }}
        .footer p {{
            color: #718096;
            font-size: 13px;
            margin-bottom: 8px;
        }}
        .social-links a {{
            color: #667eea;
            text-decoration: none;
            margin: 0 8px;
            font-size: 13px;
        }}
        .note {{
            background: #d4edda;
            border-left: 4px solid #00d4aa;
            padding: 15px 20px;
            margin-top: 25px;
            border-radius: 0 8px 8px 0;
            font-size: 14px;
            color: #155724;
        }}
        .security-badge {{
            display: inline-block;
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
            color: white;
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 600;
            margin-top: 10px;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>PAY<span>VAULT</span></div>
            <div class='tagline'>Your Secure Financial Partner</div>
        </div>
        <div class='content'>
            <div class='greeting'>Hello, {name}!</div>
            <div class='message'>
                Welcome to <strong>PayVault</strong>! Thank you for choosing us as your trusted payment platform.
                <br><br>
                To activate your account and start managing your finances securely, please confirm your account by clicking the button below.
            </div>
            <div style='text-align: center;'>
                <a href='{confirmationLink}' class='button'>Confirm My Account</a>
            </div>
            <div class='note'>
                🔒 Your security is our priority. This confirmation link will expire in 24 hours. 
                <br>If you didn't create an account with PayVault, please ignore this email.
            </div>
            <div style='text-align: center; margin-top: 20px;'>
                <span class='security-badge'>Bank-Grade Security</span>
            </div>
        </div>
        <div class='footer'>
            <p>Questions? Contact us at <strong>support@payvault.com</strong></p>
            <p>PayVault | Secure. Reliable. Yours.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}