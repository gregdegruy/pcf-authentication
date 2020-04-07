import * as React from 'react';

import { AuthenticationForm } from './src/AuthenticationForm';
import { PredictiveContent } from './src/PredictiveContent';

var styles = {
    debug: {
        border: "1px solid red",
        margin: "2px"
    }
};

export class App extends React.Component {
    render() {
        return (
        <>
            <div style={styles.debug}>
                <AuthenticationForm/>
            </div>
            <div style={styles.debug}>
                <PredictiveContent/>
            </div>
        </>
        );
    }
}

export default App;
