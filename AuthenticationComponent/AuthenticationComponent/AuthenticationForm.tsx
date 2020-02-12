import * as React from "react";
import { 
	DefaultButton, 
	IStackProps,
	Stack, 
	TextField
} from 'office-ui-fabric-react';

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
	
	constructor(props: any) {
		super(props)
	}

	public render(): JSX.Element {
		return (			
			<>
				<div className="ms-Grid" dir="rtl">
					<div className="ms-Grid-row">
						<h3>React version in control: {React.version}</h3>
						<h3>React version in host window: {(window as any).React.version}</h3>
						<div className={"control"}>
							<Stack {...columnProps}>
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
								<DefaultButton 
									text="Submit" 
									onClick={this.buttonClick} 
									allowDisabledFocus 
								/>
							</Stack>
						</div>
					</div>
				</div>
			</>
		);
	}

	// onChange?: (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => void;
	private usernameOnChange = (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
		this.setState({ username: newValue || '' });
	};
	private passwordOnChange = (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
		this.setState({ password: newValue || '' });
	};
	private buttonClick(): void {
		alert('Clicked with user' + this.state.username + 'and pas ' + this.state.password);
	
	};
}