import * as React from 'react';

import { AuthenticationForm } from './src/AuthenticationForm';
import { PredictiveContent } from './src/PredictiveContent';
import { OpenIdManager } from "./src/OpenIdManager";


var styles = {
    debug: {
        border: "1px solid red",
        margin: "2px"
    },
    hidden: {
        display: "none"
    },
    visible: {}
};

export class App extends React.Component {

    public readonly openId = new OpenIdManager().getInstance();	
    
    render() {
        return (
            <>
                <div style={styles.debug}>
                    <AuthenticationForm/>
                </div>
                <div style={this.openId.authenticated ? styles.visible : styles.hidden}>
                    <PredictiveContent/>
                </div>
            </>
        );
    }
}

export default App;
