import * as React from "react";
import { Dropdown, IDropdownOption } from "office-ui-fabric-react/lib/Dropdown";
import { PersonaSize } from "office-ui-fabric-react/lib/Persona";
import { setIconOptions } from "office-ui-fabric-react/lib/Styling";

setIconOptions({
	disableWarnings: true,
});

export interface IFacepileBasicExampleProps {
	numberOfFaces?: number;
	numberFacesChanged?: (newValue: number) => void;
}

export interface IFacepileBasicExampleState extends React.ComponentState, IFacepileBasicExampleProps {
	personaSize: PersonaSize;
	imagesFadeIn: boolean;
}

export class FacepileBasicExample extends React.Component<IFacepileBasicExampleProps, IFacepileBasicExampleState> {
	constructor(props: IFacepileBasicExampleProps) {
		super(props);

		this.state = {
			numberOfFaces: props.numberOfFaces || 3,
			imagesFadeIn: true,
			personaSize: PersonaSize.size32
		};
	}

	public componentWillReceiveProps(newProps: IFacepileBasicExampleProps): void {
		this.setState(newProps);
	}

	public render(): JSX.Element {
		return (
			<div className={"msFacepileExample"}>
				<h3>React version in control: {React.version}</h3>
				<h3>React version in host window: {(window as any).React.version}</h3>
				<div className={"control"}>
					<Dropdown
						label="Persona Size:"
						selectedKey={this.state.personaSize}
						options={[
							{ key: PersonaSize.size16, text: "16px" },
							{ key: PersonaSize.size24, text: "24px" },
							{ key: PersonaSize.size28, text: "28px" },
							{ key: PersonaSize.size32, text: "32px" },
							{ key: PersonaSize.size40, text: "40px" },
						]}
						onChange={this.onChangePersonaSize}
					/>
				</div>
			</div>
		);
	}

	private onChangePersonaSize = (event: React.FormEvent<HTMLDivElement>, value?: IDropdownOption): void => {
		this.setState(
			(prevState: IFacepileBasicExampleState): IFacepileBasicExampleState => {
				prevState.personaSize = value ? (value.key as PersonaSize) : PersonaSize.size32;
				return prevState;
			}
		);
	};
}
