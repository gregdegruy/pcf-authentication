import * as React from "react";
import { 
    Button, 
    BaseButton, 
	PrimaryButton,
	Stack,
} from 'office-ui-fabric-react';

import { OpenIdManager } from "./OpenIdManager";

export class AuthenticationForm extends React.Component<{}> {
	
	public readonly openId = new OpenIdManager().getInstance();		
	
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

	private buttonClicked = (event: React.MouseEvent<HTMLAnchorElement | HTMLButtonElement | HTMLDivElement | BaseButton | Button | HTMLSpanElement, MouseEvent>) => {
		this.openId.getToken();				
	};
}
