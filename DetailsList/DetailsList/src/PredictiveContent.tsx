import * as React from "react";
import * as https from "https";
import { initializeIcons } from '@uifabric/icons';
import { 
    getTheme,
    mergeStyleSets,
    Button, 
    BaseButton, 
    DetailsList, 
    DetailsListLayoutMode, 
    FontWeights,
    IColumn, 
    Icon,
    IconButton,
    IIconProps,
    Link,
    Modal,
    PrimaryButton,
} from 'office-ui-fabric-react';
import { OpenIdManager } from "./OpenIdManager";

import * as env from "../../../env/env.json";

declare const Xrm: any;

export interface ITextFieldControlledExampleState {
    iframeSource: string;
    isModalOpen: boolean;    
    items: IDetailsListBasicExampleItem[];    
}
export interface IDetailsListBasicExampleItem {
	name: string;
	repository: string;
	scorePoints: string;
	format: any;
    applicationUrl: any;
}

const fileIcons: any = {
     "docx": "WordDocument" ,
     "pdf": "PDF" ,
     "pptx": "PowerPointDocument" ,
     "xls": "WordDocument"
};

const theme = getTheme();
const cancelIcon: IIconProps = { iconName: 'Cancel' };
const iconButtonStyles = {
    root: {
        color: theme.palette.neutralPrimary,
        marginLeft: 'auto',
        marginTop: '4px',
        marginRight: '2px',
    },
    rootHovered: {
        color: theme.palette.neutralDark,
    },
};
const contentStyles = mergeStyleSets({
    container: {
      display: 'flex',
      flexFlow: 'column nowrap',
      alignItems: 'stretch',
      height: 1000,
      width: 800
    },
    header: [
      theme.fonts.xLargePlus,
      {
        flex: '1 1 auto',
        borderTop: `4px solid ${theme.palette.themePrimary}`,
        color: theme.palette.neutralPrimary,
        display: 'flex',
        alignItems: 'center',
        fontWeight: FontWeights.semibold,
        padding: '12px 12px 14px 24px',
        position: 'fixed',
        background: 'white',
        width: '750px'
      },
    ]
  });

export class PredictiveContent extends React.Component<{}, ITextFieldControlledExampleState, IDetailsListBasicExampleItem> {
	
    public state: ITextFieldControlledExampleState 
        = { items: [], isModalOpen: false, iframeSource: "" };		
	public readonly openId = new OpenIdManager().getInstance();		
	private _allItems: IDetailsListBasicExampleItem[];
    private _columns: IColumn[];        
    
	constructor(props: any) {
		super(props);				
		initializeIcons();
		this._allItems = [];
		for (let i = 0; i < 20; i++) {
			this._allItems.push({
				name: "name " + i,
				repository: "repository " + i,
				scorePoints: "scorePoints " + i,
				format: <div><Icon iconName={'PDF'} /> {"format " + i}</div>,
                applicationUrl: "applicationUrl " + i
			});
		}		
		this._columns = [
			{ key: "column1", name: "Name", fieldName: "name", minWidth: 100, maxWidth: 200, isResizable: true },
			{ key: "column2", name: "Repository", fieldName: "repository", minWidth: 100, maxWidth: 200, isResizable: true },
			{ key: "column3", name: "Score points", fieldName: "scorePoints", minWidth: 100, maxWidth: 200, isResizable: true },
			{ key: "column4", name: "Format", fieldName: "format", minWidth: 100, maxWidth: 200, isResizable: true }			,
            { key: "column5", name: "Application Url", fieldName: "applicationUrl", minWidth: 100, maxWidth: 200, isResizable: true },
		];
		this.state = {
            items: this._allItems,
            isModalOpen: false,
            iframeSource: ""
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
                            <Modal
                                titleAriaId={"9d803647-cd3a-4931-a30a-1a8464457d9e"}
                                isOpen={this.state.isModalOpen}
                                onDismiss={this.hideModal}
                                isBlocking={false}
                                containerClassName={contentStyles.container}>
                                <div className={contentStyles.header}>
                                        <span>Content</span>
                                        <IconButton
                                            styles={iconButtonStyles}
                                            iconProps={cancelIcon}
                                            ariaLabel="Close popup modal"
                                            onClick={this.hideModal}/>                                                               
                                </div>
                                <iframe src={this.state.iframeSource}
                                    frameBorder="none"
                                    width="100%"
                                    height="100%"
                                    margin-top="70px">                                        
                                </iframe>
                            </Modal>                            
						</div>
					</div>
				</div>
			</>
		);
    }

    private getPredictiveContent = (event: React.MouseEvent<HTMLAnchorElement | HTMLButtonElement | HTMLDivElement | BaseButton | Button | HTMLSpanElement, MouseEvent>) => {
        const predictiveContentId = "Flow"; // case matters!
        const contextId = "optional";

        let userPromise = new Promise((resolve, reject) => {                        		
            var token = "";
            var userSettings = Xrm.Utility.getGlobalContext().userSettings;
            Xrm.WebApi.retrieveRecord("systemuser", userSettings.userId.slice(1,-1), "?$select=seismic_cc_token").then(
                function success(result: any) {
                    console.log("Retrieved token: " + result.seismic_cc_token);
                    token = result.seismic_cc_token;
                    resolve(token);
                },
                function (error: any) {
                    console.log(error.message);
                }
            );
        });
                
        let contentPromise = new Promise((resolve, reject) => {                          		
            userPromise.then((token: any) => {
                let options = {
                    "method": "GET",
                    "hostname": env.api.toString(),
                    "path": "/predictiveContent/" + predictiveContentId + '/' + contextId,
                    "headers": {
                        "authorization": "Bearer " + token // this.openId.bearerToken
                    }
                };
                let req = https.request(options, function (response) {
                    var data = "";
                    response.on("data", function (chunk) {
                        data += chunk;
                    });		  
                    response.on("end", function () {								
                        let formattedData = JSON.parse(data);
                        resolve(formattedData);
                    });		  
                    response.on("error", function (error) {
                        console.error(error);
                        reject();
                    });
                });		  
                req.end();
            })            
        }); 		
        contentPromise.then((formattedData: any) => {
            var predictiveContent: any[] = [];
            for (let i = 0; i < formattedData.length; i++) {
                console.log(formattedData[i].applicationUrls[0].name); // handle empty
                predictiveContent.push({
                    name: formattedData[i].name,
                    repository: formattedData[i].repository,
                    scorePoints: formattedData[i].score.points,
                    format: 
                        <div>
                            <Icon iconName={fileIcons[formattedData[i].format.toLowerCase()]} />&nbsp; 
                            {formattedData[i].format}
                        </div>,
                    applicationUrl:  
                    <Link
                        key={i}
                        onClick={() => {                             
                            this.setState({iframeSource: formattedData[i].applicationUrls[0].url})
                            this.hideModal();
                        }}>
                        {formattedData[i].applicationUrls[0].name} 
                        <Icon iconName={'NavigateExternalInline'} />
                    </Link>
                });
            }
            this.setState({items: predictiveContent});            
        });
    };
    
    private hideModal = () => {
        this.setState({isModalOpen: !this.state.isModalOpen});
    }
}
