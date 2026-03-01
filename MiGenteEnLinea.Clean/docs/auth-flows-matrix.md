# Auth Flows Matrix

## Scope
- API: `src/Presentation/MiGenteEnLinea.API/Controllers/AuthController.cs`
- Web: `src/Presentation/MiGenteEnLinea.Web/Views/Auth/*`
- Shared JS: `src/Presentation/MiGenteEnLinea.Web/wwwroot/js/Custom.js`, `api-endpoints.js`

## Flows
| Flow | API Endpoint | Web View | Request Contract | Expected Result |
|---|---|---|---|---|
| Login | `POST /api/auth/login` | `Views/Auth/Login.cshtml` | `email,password,ipAddress?` | 200 + tokens, 401 invalid credentials |
| Register (Legacy) | `POST /api/auth/register` | `Views/Auth/Registrar.cshtml` | `email,nombre,apellido,tipo,telefono1,telefono2?,host,password?` | 201 + activation email |
| Activate | `POST /api/auth/activate` | `Views/Auth/Activar.cshtml` | `userId,email,password,confirmPassword` | 200 account activated |
| Resend activation | `POST /api/auth/resend-activation` | N/A (API/ops) | `email,host,userId?` | 200/404 based on state |
| Forgot password | `POST /api/auth/forgot-password` | `Views/Auth/Login.cshtml` | `email` | 200 neutral response |
| Reset password | `POST /api/auth/reset-password` | `Views/Auth/ResetPassword.cshtml` | `email,token,newPassword` | 200 reset success |
| Change password | `POST /api/auth/change-password` | API consumer authenticated | `email,userId,currentPassword,newPassword` | 200 changed, 400 invalid current |
| Change password by id | `POST /api/auth/change-password-by-id` | Admin only | `credencialId,newPassword` | 200/404 |
| Refresh | `POST /api/auth/refresh` | Token refresh client flow | `refreshToken` | 200 new tokens |
| Revoke | `POST /api/auth/revoke` | Logout server-side optional | `refreshToken` | 200 revoked |
| Delete user (soft) | `POST /api/auth/delete-user` | Admin flow | `userId,credencialId` | 200/404 |

## Key alignment decisions
- Legacy registration is preserved: password is optional in register and required in activation.
- Error payloads are serialized as camelCase from global middleware.
- Auth links for emails are generated from `AuthLinks:PublicWebBaseUrl`.
