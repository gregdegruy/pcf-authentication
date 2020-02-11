import * as React from "react";
import { DefaultButton, 
	IStackTokens,
	Stack, 
	TextField
} from 'office-ui-fabric-react';
import { PersonaSize } from "office-ui-fabric-react/lib/Persona";

const stackTokens: IStackTokens = { childrenGap: 40 };

export class AuthenticationForm extends React.Component {
	constructor(props: any) {
		super(props);

		this.state = {
			numberOfFaces: props.numberOfFaces || 3,
			imagesFadeIn: true,
			personaSize: PersonaSize.size32
		};
	}

	public componentWillReceiveProps(newProps: any): void {
		this.setState(newProps);
	}

	public render(): JSX.Element {
		return (
			<div className={"msFacepileExample"}>
				<h3>React version in control: {React.version}</h3>
				<h3>React version in host window: {(window as any).React.version}</h3>
				<div className={"control"}>
					<Stack horizontal tokens={stackTokens}>
						<TextField 
							label="Username" 
							type="text"
						/>
						<TextField 
							label="Password"
							type="password" 
						/>
						<DefaultButton 
							text="Submit" 
							onClick={_alertClicked} 
							allowDisabledFocus 
						/>
					</Stack>
				</div>
			</div>
		);
	}

}

function _alertClicked(): void {
	alert('Clicked');
}