import React, { Component } from 'react';

export class SwitchControl extends Component {
    static displayName = SwitchControl.name;

    constructor(props) {
        super(props);
        this.state = { switchState: false, nextStateChange: new Date() };
        this.UpdateStateChangeDisplay = this.UpdateStateChangeDisplay.bind(this);
        this.handleResponse = this.handleResponse.bind(this);
        this.OnInputChange = this.OnInputChange.bind(this);
        this.StartCycle = this.StartCycle.bind(this);
    }

    componentDidMount() {
        this.UpdateStateChangeDisplay();
    }

    UpdateStateChangeDisplay() {
        fetch('switch/state')
            .then(this.handleResponse);
    }

    handleResponse(response) {
        if (response.ok) {
            response.json().then(data => this.setState({ nextStateChange: data.nextChangeAt, switchState: data.currentState }));
        }

        setTimeout(this.UpdateStateChangeDisplay, 1000);
    }

    SwitchOff() {
        fetch('switch/off').then(r => r.json());
    }

    SwitchOn() {
        fetch('switch/on').then(r => r.json());
    }

    StartCycle() {
        fetch('switch/start', {
            method: 'post',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ "patternBlob": this.state.patternString })
        }).then(r => r.json());
    }

    OnInputChange(element) {
        this.setState({ patternString: element.currentTarget.value});
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
                <p>
                    <input className="txt txt-primary" id="txtPattern" value={this.state.patternString} onChange={(element) => this.OnInputChange(element)} />
                    <button className="btn btn-primary" onClick={this.StartCycle}>Start</button>
                </p>
            </div>
        );
    }
}
