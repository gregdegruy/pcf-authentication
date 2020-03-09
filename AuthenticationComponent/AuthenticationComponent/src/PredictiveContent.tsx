import * as React from "react";
import * as https from "https";
import { Button, BaseButton, PrimaryButton } from 'office-ui-fabric-react/lib/Button';
import { DetailsList, DetailsListLayoutMode, IColumn } from 'office-ui-fabric-react/lib/DetailsList';

import { OpenIdManager } from "./OpenIdManager";

import * as env from "../../../env/env.json";

export interface ITextFieldControlledExampleState {
	items: IDetailsListBasicExampleItem[];
}

export interface IDetailsListBasicExampleItem {
	name: string;
	repository: string;
	scorePoints: string;
	format: string;
	applicationUrl: string;
}

export class PredictiveContent extends React.Component<{}, ITextFieldControlledExampleState, IDetailsListBasicExampleItem> {
	
	public state: ITextFieldControlledExampleState = { items: [] };		
	public readonly openId = new OpenIdManager().getInstance();		
	private _allItems: IDetailsListBasicExampleItem[];
	private _columns: IColumn[];
	
	constructor(props: any) {
		super(props);				
		
		this._allItems = [];
		for (let i = 0; i < 20; i++) {
			this._allItems.push({
				name: "name " + i,
				repository: "repository " + i,
				scorePoints: "scorePoints " + i,
				format: "format " + i,
				applicationUrl: "applicationUrl " + i
			});
		}		
		this._columns = [
			{ key: "column1", name: "Name", fieldName: "name", minWidth: 100, maxWidth: 200, isResizable: true },
			{ key: "column2", name: "Repository", fieldName: "repository", minWidth: 100, maxWidth: 200, isResizable: true },
			{ key: "column3", name: "Score points", fieldName: "scorePoints", minWidth: 100, maxWidth: 200, isResizable: true },
			{ key: "column4", name: "Format", fieldName: "format", minWidth: 100, maxWidth: 200, isResizable: true }			,
			{ key: "column4", name: "Application Url", fieldName: "applicationUrl", minWidth: 100, maxWidth: 200, isResizable: true }
		];
		this.state = {
			items: this._allItems
		};
	}

	public render(): JSX.Element {
		return (			
			<>
				<div className="ms-Grid">				
					<div className="ms-Grid-row">
						<div className="ms-Grid-col ms-sm6 ms-md8 ms-lg10">							
                            <PrimaryButton 
									text="GET Predictive Content" 
									onClick={this.predictiveContent}
									allowDisabledFocus 
									/>
                            <DetailsList
								items={this.state.items}
								columns={this._columns}
								setKey="set"
								layoutMode={DetailsListLayoutMode.justified}
								selectionPreservedOnEmptyClick={true}
								ariaLabelForSelectionColumn="Toggle selection"
								ariaLabelForSelectAllCheckbox="Toggle selection for all items"
								checkButtonAriaLabel="Row checkbox"
							/>
						</div>
					</div>
				</div>
			</>
		);
	}

	private predictiveContent = (event: React.MouseEvent<HTMLAnchorElement | HTMLButtonElement | HTMLDivElement | BaseButton | Button | HTMLSpanElement, MouseEvent>) => {
		const predictiveContentId = "Flow"; // case matters!
		const contextId = "optional";
		let options = {
			"method": "GET",
			"hostname": env.api.toString(),
			"path": "/predictiveContent/" + predictiveContentId + '/' + contextId,
			"headers": {
				"authorization": "Bearer " + this.openId.token
			}
		};		  
		let req = https.request(options, function (response) {
			var data = "";
			response.on("data", function (chunk) {
				data += chunk;
			});		  
			response.on("end", function () {								
				let predictiveContent = JSON.parse(data);
				// this._allItems = [];
				for (let i = 0; i < predictiveContent.length; i++) {
					console.log(predictiveContent[i].name);
					console.log(predictiveContent[i].repository);
					console.log(predictiveContent[i].score.points);
					console.log(predictiveContent[i].format);
					console.log(predictiveContent[i].applicationUrls[0].name); // handle empty
					console.log(predictiveContent[i].applicationUrls[0].url);
					// console.log(JSON.stringify(payload[i]));
					// this._allItems.push({
					// 	title: "title " + payload[i].title,
					// 	name: "name " + payload[i].name,
					// 	systemType: "systemType " + payload[i].systemType,
					// 	contextType: "contextType " + payload[i].contextType
					// });
				}

				// this.setState({items: payload});
			});		  
			response.on("error", function (error) {
				console.error(error);
			});
		});		  
		req.end();
	};
}
