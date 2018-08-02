using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using WebApi.Providers;
using WebApi.Models;
using Microsoft.Owin.Security.Facebook;
using WebApi.Facebook;

namespace WebApi
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // Pour plus d'informations sur la configuration de l'authentification, visitez https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configurer le contexte de base de données et le gestionnaire des utilisateurs pour utiliser une instance unique par demande
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Activer l'application pour utiliser un cookie afin de stocker les informations de l'utilisateur connecté
            // et utiliser un cookie pour stocker temporairement des informations sur un utilisateur qui se connecte avec un fournisseur de connexion tiers
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configurer l'application pour le flux basé sur OAuth
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromHours(1),
                // Jeu de modes en production AllowInsecureHttp = false
                AllowInsecureHttp = true
            };

            // Activer l'application pour utiliser les jetons du porteur afin d'authentifier les utilisateurs
            app.UseOAuthBearerTokens(OAuthOptions);

            // Décommenter les lignes suivantes pour activer la connexion avec des fournisseurs de connexion tiers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");

            var facebookOptions = new FacebookAuthenticationOptions()
            {
                 AppId = "206835183317829",
                 AppSecret=  "a444c68654bd680ff98c0c21344a13e6",
                 BackchannelHttpHandler = new FacebookBackChannelHandler(),
                UserInformationEndpoint = "https://graph.facebook.com/v2.4/me?fields=id,email"
            };

            facebookOptions.Scope.Add("email");
            app.UseFacebookAuthentication(facebookOptions);


            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "247221902709-mtcgms7r87ue6iasdbni6jklu7hgq4q7.apps.googleusercontent.com",
                ClientSecret = "xXidnDSckepcWDllfYkiqNbZ"
            });
        }
    }
}
