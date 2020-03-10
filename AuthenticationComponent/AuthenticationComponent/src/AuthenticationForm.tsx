import * as React from "react";
import { Depths } from '@uifabric/fluent-theme/lib/fluent/FluentDepths';
import { 
    Button, 
    BaseButton, 
	PrimaryButton,
	Stack,
	TextField
} from 'office-ui-fabric-react';

import { OpenIdManager } from "./OpenIdManager";

export interface ITextFieldControlledExampleState {
	username: string;
	password: string;
}

export class AuthenticationForm extends React.Component<{}, ITextFieldControlledExampleState> {
	
	public state: ITextFieldControlledExampleState = { username: "", password: "" };		
	public readonly openId = new OpenIdManager().getInstance();		
	
	constructor(props: any) {
		super(props);
		this.state = {
			username: "",
			password: ""
		};
	}

	public render(): JSX.Element {
		return (			
			<>
				<div className="ms-Grid">				
					<div className="ms-Grid-row">
						<div className="ms-Grid-col ms-sm6 ms-md4 ms-lg2">
							<div style={{ boxShadow: Depths.depth8 }}>
								<h3>React version in control: {React.version}</h3>
								<br/>
								<h3>React version in host window: {(window as any).React.version}</h3>
							</div>/>
						</div>
						<div className="ms-Grid-col ms-sm6 ms-md8 ms-lg10">
							<Stack>
								<TextField 
									label="Username" 
									type="text"
									value={this.state.username}
									onChange={this.usernameOnChange}
									disabled={this.openId.authenticated}
									/>
								<TextField 
									label="Password"
									type="password" 
									value={this.state.password}
									onChange={this.passwordOnChange}
									disabled={this.openId.authenticated}
									/>
								<PrimaryButton 
									text={this.openId.authenticated ? "Sign Out" : "Sign In"} 
									onClick={this.buttonClicked}
									disabled={this.openId.authenticated}
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
		this.setState({ username: newValue || "" });
	};

	private passwordOnChange = (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
		this.setState({ password: newValue || "" });
	};

	private buttonClicked = (event: React.MouseEvent<HTMLAnchorElement | HTMLButtonElement | HTMLDivElement | BaseButton | Button | HTMLSpanElement, MouseEvent>) => {
		this.openId.getToken();				
	};
}
