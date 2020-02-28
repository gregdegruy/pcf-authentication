import { Button, BaseButton, PrimaryButton } from 'office-ui-fabric-react/lib/Button';
import { UserManager, WebStorageStateStore, Log } from "oidc-client";
import * as React from "react";
import { Stack, IStackProps } from 'office-ui-fabric-react/lib/Stack';
import { TextField } from 'office-ui-fabric-react/lib/TextField';
import * as env from "../../env/env.json";

const columnProps: Partial<IStackProps> = {
	tokens: { childrenGap: 15 },
	styles: { root: { width: 600 } }
};

export interface ITextFieldControlledExampleState {
	username: string;
	password: string;
}

export class AuthenticationForm extends React.Component<{}, ITextFieldControlledExampleState> {
	
	public state: ITextFieldControlledExampleState = { username: '', password: '' };
	
	public userManager: UserManager;

	constructor(props: any) {
		super(props);
		
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
	}

	public render(): JSX.Element {
		return (			
			<>
				<div className="ms-Grid">				
					<div className="ms-Grid-row">
						<div className="ms-Grid-col ms-sm6 ms-md4 ms-lg2">
							<h3>React version in control: {React.version}</h3>
							<h3>React version in host window: {(window as any).React.version}</h3>
						</div>
						<div className="ms-Grid-col ms-sm6 ms-md8 ms-lg10">
							<Stack>
								<TextField 
									label="Username" 
									type="text"
									value={this.state.username}
									onChange={this.usernameOnChange}
									/>
								<TextField 
									label="Password"
									type="password" 
									value={this.state.password}
									onChange={this.passwordOnChange}
									/>
								<PrimaryButton 
									text="Submit" 
									onClick={this.buttonClicked} 
									allowDisabledFocus 
									/>
							</Stack>
						</div>
					</div>
				</div>
			</>
		);
	}

	private usernameOnChange = (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
		this.setState({ username: newValue || '' });
	};

	private passwordOnChange = (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
		this.setState({ password: newValue || '' });
	};

	private buttonClicked = (event: React.MouseEvent<HTMLAnchorElement | HTMLButtonElement | HTMLDivElement | BaseButton | Button | HTMLSpanElement, MouseEvent>) => {
		alert('usr  is ' + this.state.username);
		alert('pass is ' + this.state.password);		

		this.userManager.signinPopup({
			popupWindowFeatures : 'location=no,toolbar=no,width=680,height=700'
		})
		.catch(function(error) {
			console.error('error while logging in through the popup', error);
		});
	};
}
