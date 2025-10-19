BASIC POINTS NEEDED FOR TOKEN & AUTHERIZATION

Here are the 7 main registered claims, explained one by one:

**1. iss (Issuer)**
**What it is**: Who created and signed this token.
**Example**: "iss": "https://myapp.com"
**When to use**: When you want to know which server or service issued the token.
**Purpose**: Helps the receiver trust the token only if it comes from a known issuer.
**Without it**: You won’t know who made the token—could be risky if you accept tokens from unknown sources.
**With it**: Your app can reject tokens not issued by you (e.g., if someone tries to fake a token).

**2. sub (Subject)**
**What it is**: Who the token is about (usually the user).
**Example**: "sub": "user123" or "sub": "john@example.com"
**When to use**: Almost always—this identifies the user or entity the token belongs to.
**Purpose**: Tells your app which user is making the request.
**Without it**: You won’t know whose data or permissions to use.
**With it**: You can safely fetch user data, check roles, etc.

**3. aud (Audience)**
**What it is**: Who the token is intended for (which service or app should accept it).
**Example**: "aud": "https://api.myapp.com" or "aud": "mobile-app"
**When to use**: When your token might be used by multiple services, and you want to restrict usage.
**Purpose**: Prevents a token meant for your mobile app from being used on your admin dashboard.
**Without it**: Any service that trusts your token signature could use it—even if it wasn’t meant for them.
**With it**: Services can check: “Is this token for me?” If not, reject it.

**4. exp (Expiration Time)**
**What it is**: When the token stops being valid (in Unix timestamp).
**Example**: "exp": 1730000000 → means valid until that second.
**When to use**: Always for security—tokens shouldn’t last forever.
**Purpose**: Limits how long a stolen token can be misused.
**Without it**: Token never expires → big security risk!
**With it**: After the time passes, the token is rejected automatically.

**5. nbf (Not Before)**
**What it is**: The time before which the token is not valid.
**Example**: "nbf": 1730000000 → token only works after this time.
**When to use**: Rarely—but useful for scheduled access (e.g., a user gets access starting tomorrow).
**Purpose**: Delays token validity.
**Without it**: Token is valid immediately after creation.
**With it**: You can issue a token today that only works next week.

**6. iat (Issued At)**
**What it is**: When the token was created (Unix timestamp).
**Example**: "iat": 1729996400
**When to use**: Helpful for logging, debugging, or calculating token age.
**Purpose**: Lets you know how old the token is.
**Without it**: You can’t tell when it was made (but exp often makes this optional).
**With it**: You can enforce rules like “reject tokens older than 1 hour” even if not expired.

**7. jti (JWT ID)**
**What it is**: A unique ID for the token (like a serial number).
**Example**: "jti": "abc123xyz789"
**When to use**: When you need to track or revoke specific tokens (e.g., logout).
**Purpose**: Prevents replay attacks and allows token blacklisting.
**Without it**: Hard to revoke a single token—you’d have to wait for it to expire.
**With it**: You can store used jtis and reject duplicates or revoked ones.

🔑 JWT Authentication in ASP.NET Core – Key Points
Secure JWT token generation (for login/auth).
Token-based authentication for protected API endpoints using [Authorize].
⚙️ 2. Configuration (appsettings.json)
Make sure your **appsettings.json** includes:

{
  "JWT": {
    "Key": "YourSuperSecureSecretKey32BytesLong!",  // Must be ≥ 32 chars(you can GUID here like GUID.NewGUID())
    "Issuer": "YourAppName",
    "Audience": "YourAppUsers"
  }
}
🧩 3. Token Generation
Tokens are generated using JwtSecurityToken + JwtSecurityTokenHandler.
Includes standard claims:
sub (subject/user ID)
jti (unique token ID)
iss (issuer)
aud (audience)
exp (expiration, e.g., 20–60 mins)
Signed with HMAC-SHA256 using the secret key.(FOR THIS ALGORITH WE HAVE ONE CLASS '**SECURITYALGORITHS**')
🔐 4. Authentication Setup
In Program.cs:

Added AddAuthentication(JwtBearerDefaults.AuthenticationScheme) 
**JwtBearerDefaults.AuthenticationScheme Default value 'Bearer'**
Configured TokenValidationParameters with:
**[Note: If we provide Issuer,Audience etc.. during token generation then only we need to validate(need to provide 'true') other wise we can provide 'false'. By default these properties are 'True' only]
**
ValidateIssuer = true
ValidateAudience = true
ValidateIssuerSigningKey = true
Matching ValidIssuer, ValidAudience, and IssuerSigningKey
Called app.UseAuthentication() and app.UseAuthorization() in correct order.

❗ Order matters:
UseRouting() → UseAuthentication() → UseAuthorization() → MapControllers() 



🧪 6. Testing with Postman
Call your login/token endpoint → copy JWT.
For protected routes:
FirstWay : Go to Authorization tab
Type: Bearer Token
Paste your token
Send request → should return 200 OK if valid.


Second Way: Go To Header Tab
Add 'Autherization' as key
Add 'Bearer <Token>' as Value
