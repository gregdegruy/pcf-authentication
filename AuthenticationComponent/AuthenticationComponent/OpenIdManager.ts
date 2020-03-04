import { UserManager, WebStorageStateStore, Log } from "oidc-client";

import * as env from "../../env/env.json";

export class OpenIdManager {
    private static instance: OpenIdManager;
    public userManager: UserManager;
    public expirersAt: number;
    public token: string;
    constructor() {
        this.userManager = new UserManager({
			authority: env.authority,
			client_id: env.clientId,
			redirect_uri: env.redirectUris[0],
			scope: env.scope,
			response_type: "token",
			loadUserInfo: false
		});

		this.userManager.events.addUserLoaded((user) => {
            if (window.location.href.indexOf("signin-oidc") !== -1) {
				console.log(user);
                console.log("Authorization: " + user.token_type + ' ' + user.access_token);
            }
        });

        this.userManager.events.addSilentRenewError((e) => {
            console.log("silent renew error", e.message);
        });

        this.userManager.events.addAccessTokenExpired(() => {
            console.log("token expired");
        });     

        this.userManager.signinPopupCallback();
    }
    getInstance(): OpenIdManager {
        if (OpenIdManager.instance) {
            return OpenIdManager.instance;
        } else {
            OpenIdManager.instance = new OpenIdManager();
            return OpenIdManager.instance;
        }
    }
    getToken(): void {
        this.userManager
        // TODO: handle CORs
        // .signinSilent()
        .signinPopup({
            popupWindowFeatures : 'location=no,toolbar=no,width=680,height=700'
        }).then((user) =>{
            console.log(user);
            console.log("user" + JSON.stringify(user));
            this.expirersAt = user.expires_at;
            this.token = user.access_token;
        })
        .catch(function(error) {
            console.error('error while logging in through the popup', error);
        });
    }    
}
