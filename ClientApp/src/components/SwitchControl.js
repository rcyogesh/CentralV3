import React, { Component } from 'react';

export class SwitchControl extends Component {
    static displayName = SwitchControl.name;

    constructor(props) {
        super(props);
        this.state = { switchState: false, nextStateChange: new Date() };

        //this.UpdateState = this.UpdateState.bind(this);
        this.UpdateStateChangeDisplay = this.UpdateStateChangeDisplay.bind(this);
        //this.UpdateState();
    }

    componentDidMount() {
        //this.UpdateState();
        this.UpdateStateChangeDisplay();
    }

    //UpdateState() {
    //    fetch('switch/value')
    //        .then(response => response.json())
    //        .then(data => {
    //            this.setState({ switchState: data });
    //        });
    //            setTimeout(this.UpdateState, 1000);
    //}

    UpdateStateChangeDisplay() {
        fetch('switch/state')
            .then(response => response.json())
            .then(data => {
                this.setState({ nextStateChange: data.nextChangeAt, switchState: data.currentState });
            });
        setTimeout(this.UpdateStateChangeDisplay, 1000);
    }

    SwitchOff() {
        fetch('switch/off').then(r => r.json());
    }

    SwitchOn() {
        fetch('switch/on').then(r => r.json());
    }

    render() {
        let contents = this.state.switchState
            ? <p><em>Switch is ON</em></p>
            : <p>Switch is OFF</p>;

        let nextChangeContent = "";
        if (this.state.nextStateChange != null) {
            let date = new Date(this.state.nextStateChange);
            nextChangeContent = date.toLocaleTimeString();
        }

        return (
            <div>
                <h1>Switch</h1>
                {contents}
                <p>Next state change at {nextChangeContent}. Time now is {new Date().toLocaleTimeString()}</p>

                <button className="btn btn-primary" onClick={this.SwitchOn}>On</button>
                <p></p>
                <button className="btn btn-primary" onClick={this.SwitchOff}>Off</button>
            </div>
        );
    }
}
