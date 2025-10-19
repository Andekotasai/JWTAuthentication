# JWT Authentication (ASP.NET Core)

A concise guide to JWT claims, secure token generation, and how to configure JWT authentication in an ASP.NET Core application.

---

## Overview

This document explains the important JWT (JSON Web Token) registered claims and provides examples and key points for implementing JWT authentication in ASP.NET Core: generating tokens, configuring authentication, and testing with Postman.

---

## Registered JWT Claims (main ones)

These are the seven standard claims commonly used in JWTs:

1. iss (Issuer)
   - What it is: Who created and signed the token.
   - Example: `"iss": "https://myapp.com"`
   - Use: To verify the token comes from a trusted issuer.
   - Benefit: Reject tokens not issued by you.

2. sub (Subject)
   - What it is: Who the token is about (usually the user).
   - Example: `"sub": "user123"` or `"sub": "john@example.com"`
   - Use: Identify the user or entity the token represents.

3. aud (Audience)
   - What it is: Who the token is intended for (which service/app).
   - Example: `"aud": "https://api.myapp.com"` or `"aud": "mobile-app"`
   - Use: Prevent tokens intended for one service from being accepted by another.

4. exp (Expiration Time)
   - What it is: When the token stops being valid (Unix timestamp).
   - Example: `"exp": 1730000000`
   - Use: Always set — limits risk from stolen tokens.

5. nbf (Not Before)
   - What it is: Time before which the token is not valid.
   - Example: `"nbf": 1730000000`
   - Use: Useful for scheduled future access.

6. iat (Issued At)
   - What it is: When the token was issued (Unix timestamp).
   - Example: `"iat": 1729996400`
   - Use: Logging, debugging, or enforcing token age rules.

7. jti (JWT ID)
   - What it is: Unique token identifier (like a serial number).
   - Example: `"jti": "abc123xyz789"`
   - Use: Enable token revocation, prevent replay attacks.

---

## Key Points for ASP.NET Core JWT Authentication

- Generate secure JWT tokens (typically on login).
- Protect API endpoints using the [Authorize] attribute.
- Sign tokens using a secure algorithm (HMAC-SHA256 is common).
- Use registered claims (sub, jti, iss, aud, exp) as needed for validation and security.

---

## Configuration (appsettings.json)

Add a JWT section to your appsettings.json:

```json
{
  "JWT": {
    "Key": "YourSuperSecureSecretKey32BytesLong!",
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers",
    "ExpiryMinutes": 60
  }
}
```

Notes:
- The Key must be secure and at least 32 characters for an HMAC-based signing key.
- Replace placeholders with your real values.

---

## Token Generation (example)

Tokens are typically generated using `JwtSecurityToken` and `JwtSecurityTokenHandler`. Include these claims:

- sub (subject / user ID)
- jti (unique token ID)
- iss (issuer)
- aud (audience)
- exp (expiration)

Example (C# — simplified):

```csharp
  var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config["JWT:Key"]));

  JwtHeader header = new JwtHeader(new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
  JwtPayload payload = new JwtPayload(issuer:this._config["JWT:issuer"],audience:"",claims:new List<Claim>()
  {
      new Claim(JwtRegisteredClaimNames.Sub,"Sai"),
      new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
  }, expires: DateTime.UtcNow.AddMinutes(20),notBefore:DateTime.UtcNow);

  var token = new JwtSecurityToken(header, payload);
  return new JwtSecurityTokenHandler().WriteToken(token);
```

---

## Authentication Setup (Program.cs)

Register authentication and configure validation parameters:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience=false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:issuer"],
        IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
    };
});
```

Important note:
- If you generate tokens with Issuer/Audience, set `ValidateIssuer` and `ValidateAudience` to `true`. If you don't use these claims, you can set them to `false` (but it's safer to validate them).

---

## Middleware Order (critical)

Make sure you call middleware in this order:

1. app.UseRouting();
2. app.UseAuthentication();
3. app.UseAuthorization();
4. app.MapControllers();

Order matters — authentication must run before authorization.

---

## Testing with Postman

1. Call your login/token endpoint and copy the returned JWT.
2. For protected routes, you can send the JWT in two ways:

- Authorization tab
  - Type: Bearer Token
  - Paste your token

- Headers tab
  - Key: Authorization
  - Value: Bearer <your_token_here>

3. Send the request — a valid token returns 200 OK.

---

## Security Tips

- Keep your signing key secret and rotate keys periodically.
- Use short-lived access tokens (e.g., 20–60 minutes) and refresh tokens if needed.
- Consider storing `jti` values in a blacklist to support immediate revocation.
- Use HTTPS for all token transmission.

