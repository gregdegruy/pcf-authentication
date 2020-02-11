import { IInputs, IOutputs } from "./generated/ManifestTypes";
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { FacepileBasicExample, IFacepileBasicExampleProps } from './Facepile';

export class AuthenticationComponent
	implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	
	private _value: number;	
	private _notifyOutputChanged: () => void;	
	private labelElement: HTMLLabelElement;	
	private inputElement: HTMLInputElement;	
	private _container: HTMLDivElement;	
	private _context: ComponentFramework.Context<IInputs>;	
	private _refreshData: EventListenerOrEventListenerObject;
	private props: IFacepileBasicExampleProps = {
		numberFacesChanged: this.numberFacesChanged.bind(this),
	}

	constructor() { }

	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	) {
		this._context = context;		
		this._container = document.createElement("div");
		
		this.buildSampleComponent(context, notifyOutputChanged);
		this.buildInputForm(context);

		container.appendChild(this._container);
	}

	public buildInputForm(context: ComponentFramework.Context<IInputs>): void {
		var formElement = document.createElement("form");
		var userNameInputElement = document.createElement("input");
		userNameInputElement.setAttribute("type", "text");
		userNameInputElement.setAttribute("min", "1");
		userNameInputElement.setAttribute("max", "64");
		var passwordInputElement = document.createElement("input");
		passwordInputElement.setAttribute("type", "password");
		passwordInputElement.setAttribute("min", "1");
		passwordInputElement.setAttribute("max", "256");
		var buttonElement = document.createElement("button");
		buttonElement.setAttribute("type", "button");
		var text = document.createTextNode("Submit");
		buttonElement.appendChild(text);

		formElement.appendChild(userNameInputElement);
		formElement.appendChild(passwordInputElement);
		formElement.appendChild(buttonElement);
		this._container.appendChild(formElement);
	}

	public buildSampleComponent(context: ComponentFramework.Context<IInputs>, notifyOutputChanged: () => void): void {
		this._notifyOutputChanged = notifyOutputChanged;
		this._refreshData = this.refreshData.bind(this);

		this.inputElement = document.createElement("input");
		this.inputElement.setAttribute("type", "range");
		this.inputElement.addEventListener("input", this._refreshData);
		this.inputElement.setAttribute("min", "1");
		this.inputElement.setAttribute("max", "1000");
		this.inputElement.setAttribute("class", "linearslider");
		this.inputElement.setAttribute("id", "linearrangeinput");

		this.labelElement = document.createElement("label");
		this.labelElement.setAttribute("class", "TS_LinearRangeLabel");
		this.labelElement.setAttribute("id", "lrclabel");

		this._value = context.parameters.sliderValue.raw
			? context.parameters.sliderValue.raw
			: 0;
		this.inputElement.value =
			context.parameters.sliderValue.formatted
				? context.parameters.sliderValue.formatted
				: "0";

		this.labelElement.innerHTML = context.parameters.sliderValue.formatted
			? context.parameters.sliderValue.formatted
			: "0";

		this._container.appendChild(this.inputElement);
		this._container.appendChild(this.labelElement);
	}  

	public refreshData(evt: Event): void {
		this._value = (this.inputElement.value as any) as number;
		this.labelElement.innerHTML = this.inputElement.value;
		this._notifyOutputChanged();
	}

	public updateView(context: ComponentFramework.Context<IInputs>): void {
		this._value = context.parameters.sliderValue.raw
			? context.parameters.sliderValue.raw
			: 0;
		this._context = context;
		this.inputElement.value =

			context.parameters.sliderValue.formatted
				? context.parameters.sliderValue.formatted
				: "";

		this.labelElement.innerHTML = context.parameters.sliderValue.formatted
			? context.parameters.sliderValue.formatted
			: "";

		ReactDOM.render(
			React.createElement(
				FacepileBasicExample,
				this.props
			),
			this._container
		);
	}

	private numberFacesChanged(newValue: number) {
		if (this.props.numberOfFaces !== newValue) {
			this.props.numberOfFaces = newValue;
			this._notifyOutputChanged();
		}
	}

	public getOutputs(): IOutputs {
		return {
			sliderValue: this._value
		};
	}

	public destroy() {
		this.inputElement.removeEventListener("input", this._refreshData);
	}
}
