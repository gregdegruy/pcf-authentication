import { IInputs, IOutputs } from "./generated/ManifestTypes";

export class TSLinearInputComponent
	implements ComponentFramework.StandardControl<IInputs, IOutputs> {
	
	private _value: number;	
	private _notifyOutputChanged: () => void;	
	private labelElement: HTMLLabelElement;	
	private inputElement: HTMLInputElement;	
	private _container: HTMLDivElement;	
	private _context: ComponentFramework.Context<IInputs>;	
	private _refreshData: EventListenerOrEventListenerObject;

	constructor() { }

	public init(
		context: ComponentFramework.Context<IInputs>,
		notifyOutputChanged: () => void,
		state: ComponentFramework.Dictionary,
		container: HTMLDivElement
	) {
		this._context = context;
		this._container = document.createElement("div");
		this._notifyOutputChanged = notifyOutputChanged;
		this._refreshData = this.refreshData.bind(this);
		// creating HTML elements for the input type range and binding it to the function which refreshes the component data
		this.inputElement = document.createElement("input");
		this.inputElement.setAttribute("type", "range");
		this.inputElement.addEventListener("input", this._refreshData);
		//setting the max and min values for the component.
		this.inputElement.setAttribute("min", "1");
		this.inputElement.setAttribute("max", "1000");
		this.inputElement.setAttribute("class", "linearslider");
		this.inputElement.setAttribute("id", "linearrangeinput");
		// creating a HTML label element that shows the value that is set on the linear range component
		this.labelElement = document.createElement("label");
		this.labelElement.setAttribute("class", "TS_LinearRangeLabel");
		this.labelElement.setAttribute("id", "lrclabel");
		// retrieving the latest value from the component and setting it to the HTML elements.
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
		// appending the HTML elements to the component's HTML container element.
		this._container.appendChild(this.inputElement);
		this._container.appendChild(this.labelElement);
		container.appendChild(this._container);
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