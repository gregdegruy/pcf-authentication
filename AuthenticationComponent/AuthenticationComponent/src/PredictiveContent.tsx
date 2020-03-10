import * as React from "react";
import * as https from "https";
import { initializeIcons } from '@uifabric/icons';
import { Button, BaseButton, PrimaryButton } from 'office-ui-fabric-react/lib/Button';
import { DetailsList, DetailsListLayoutMode, IColumn } from 'office-ui-fabric-react/lib/DetailsList';
import { Icon } from 'office-ui-fabric-react/lib/Icon';


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
    public PowerPointDocumentIcon = () => <Icon iconName="PowerPointDocument" className="ms-IconExample" />;
    public PDFIcon = () => <Icon iconName="PDF" className="ms-IconExample" />;
    public NavigateExternalInlineIcon = () => <Icon iconName="NavigateExternalInline" className="ms-IconExample" />;
	
	constructor(props: any) {
		super(props);				
		initializeIcons();
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
			{ key: "column5", name: "Application Url", fieldName: "applicationUrl", minWidth: 100, maxWidth: 200, isResizable: true }
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
                            <this.PowerPointDocumentIcon />
                            <PrimaryButton 
									text="GET Predictive Content" 
									onClick={this.getPredictiveContent}
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

	private getPredictiveContent = (event: React.MouseEvent<HTMLAnchorElement | HTMLButtonElement | HTMLDivElement | BaseButton | Button | HTMLSpanElement, MouseEvent>) => {
        const predictiveContentId = "Flow"; // case matters!
        const contextId = "optional";
        let options = {
            "method": "GET",
            "hostname": env.api.toString(),
            "path": "/predictiveContent/" + predictiveContentId + '/' + contextId,
            "headers": {
                "authorization": "Bearer " + this.openId.bearerToken
            }
        };
        let contentPromise = new Promise((resolve, reject) => {                          		
            var predictiveContent: any[] = [];  
            let req = https.request(options, function (response) {
                var data = "";
                response.on("data", function (chunk) {
                    data += chunk;
                });		  
                response.on("end", function () {								
                    let formattedData = JSON.parse(data);
                    for (let i = 0; i < formattedData.length; i++) {
                        console.log(formattedData[i].name);
                        console.log(formattedData[i].repository);
                        console.log(formattedData[i].score.points);
                        console.log(formattedData[i].format);
                        console.log(formattedData[i].applicationUrls[0].name); // handle empty
                        console.log(formattedData[i].applicationUrls[0].url);
                        predictiveContent.push({
                            name: formattedData[i].name,
                            repository: formattedData[i].repository,
                            scorePoints: formattedData[i].score.points,
                            format: formattedData[i].format,
                            applicationUrl: formattedData[i].applicationUrls[0].name
                        });
                    }
                    resolve(predictiveContent);
                });		  
                response.on("error", function (error) {
                    console.error(error);
                    reject();
                });
            });		  
            req.end();
        }); 		
        contentPromise.then((payload: any) => {
            this.setState({items: payload});
        });
	};
}
