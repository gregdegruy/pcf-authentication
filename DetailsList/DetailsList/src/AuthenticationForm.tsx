import * as React from "react";
import { 
    Button, 
	BaseButton, 
	DefaultButton,
	Dialog,
	DialogFooter,
	DialogType,
	PrimaryButton,
	Stack,
	TextField
} from 'office-ui-fabric-react';

import { OpenIdManager } from "./OpenIdManager";

declare const Xrm: any;

export interface ITextFieldControlledExampleState {
	username: string;
	password: string;
	hideDialog: boolean;
}

export class AuthenticationForm extends React.Component<{}, ITextFieldControlledExampleState> {
	
	public readonly openId = new OpenIdManager().getInstance();		
	
	public state: ITextFieldControlledExampleState 
		= { username: "", password: "", hideDialog: true };

	constructor(props: any) {
		super(props);
	}

	public render(): JSX.Element {
		return (			
			<>
				<div className="ms-Grid">				
					<div className="ms-Grid-row">
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
									text={this.openId.authenticated ? "Sign Out" : "Sign In"} 
									onClick={this.buttonClicked}
									disabled={this.openId.authenticated}
									allowDisabledFocus 
									/>
							</Stack>
							<Dialog
								hidden={this.state.hideDialog}
								onDismiss={this.closeDialog}
								dialogContentProps={{
									type: DialogType.normal,
									title: 'Confirm login',
									subText: 'This will overwrite EXT_EMAIL_HERE for CDS_EMAIL. Login?',
								}}
								>
								<DialogFooter>
									<PrimaryButton onClick={this.loginConfirmed} text="Yes" />
									<DefaultButton onClick={this.closeDialog} text="No, not yet" />
								</DialogFooter>
								</Dialog>
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
		this.showDialog();
	};

	private showDialog = ():void => {
		this.setState({hideDialog: false});			
	};
	private closeDialog = ():void => {
		this.setState({hideDialog: true});			
	};
	private loginConfirmed = ():void => {
		this.setState({hideDialog: true});			
		
		// this.openId.getToken();	
		
		// store in CDS
		var globalContext = Xrm.Utility.getGlobalContext();    
		var serverURL = globalContext.getClientUrl();
		var actionName = "seismic_cc_login_action";

		var data = {
			"username": this.state.username,
			"password": this.state.password
		};

		var req = new XMLHttpRequest(); 
		req.open("POST", serverURL + "/api/data/v9.0/" + actionName, true); 
		req.setRequestHeader("Accept", "application/json"); 
		req.setRequestHeader("Content-Type", "application/json; charset=utf-8"); 
		req.setRequestHeader("OData-MaxVersion", "4.0"); 
		req.setRequestHeader("OData-Version", "4.0"); 
		req.onreadystatechange = function () { 
			if (this.readyState === 4) { 
				req.onreadystatechange = null; 

				if (this.status === 200 || this.status === 204)  { 
					var successMessage:string = "CDS PCF Login Action Executed Successfully from control...";
					console.log(successMessage);
					alert(successMessage);
					var result = JSON.parse(this.response);
					alert(result.MyOutputParameter);
				} 

				else  { 
					var error = JSON.parse(this.response).error; 
					alert("Ask your admin if the Action is activated or give the Error in Action: "+error.message);  
				} 
			} 
		}; 
		req.send(window.JSON.stringify(data)); 
	};
}
