import m from 'mithril';
import classNames from 'classnames';
import { Form } from 'powerform'

import { CheckboxGroup, Select2, OptionsField } from 'FormHelpers'
import Styles from './StaffDecisionSearchSettingsPanel.css';

export class StaffUnitSearchSettingsPowerForm extends Form {
    constructor({
        studentOptions,
        decisionStateOptions,
        unitCoordinatorOptions,
        toggleOptions,
        ...config
    }) {
        super(config)
        this.students = OptionsField.new({
            multiple: true,
            options: studentOptions
        });
        this.decisionStates = OptionsField.new({
            multiple: true,
            options: decisionStateOptions
        });
        this.unitCoordinators = OptionsField.new({
            multiple: true,
            options: unitCoordinatorOptions
        });
        this.toggles = OptionsField.new({
            options: toggleOptions
        });
        Form.new.call(() => this, config);
        this.config = config;
    }
}


export default function StaffDecisionSearchSettingsPanel() {

    const state = {};

    function oninit({ attrs }) {
        state.form = new StaffUnitSearchSettingsPowerForm(attrs);
    }

    function handleSubmit(callback, event) {
        event.preventDefault();
        event.stopPropagation();
        callback(state.form.getData());
    }

    function view({
        attrs: {
            studentOptions,
            decisionStateOptions,
            unitCoordinatorOptions,
            toggleOptions,
        }
    }) {
        console.log(CheckboxGroup());
        return (
            <form onsubmit={handleSubmit.bind(this, onsubmit)}>
                <div class="row">
                    <div class="col-lg-6 col-md-12 form-group">
                        <label for="students">Students</label>
                        <Select2
                            field={state.form.students}
                            config={{
                                multiple: true,
                                width: '100%',
                                placeholder: 'Select students to filter to...',
                                data: studentOptions.map(name => ({
                                    id: name,
                                    text: name
                                }))
                            }}
                        />
                    </div>
                    <div class="col-lg-6 col-md-12 form-group">
                        <label>Unit Coordinators</label>
                        <Select2
                            field={state.form.unitCoordinators}
                            config={{
                                multiple: true,
                                width: '100%',
                                placeholder: 'Select unit coordinators to filter to...',
                                data: unitCoordinatorOptions.map(name => ({
                                    id: name,
                                    text: name
                                }))
                            }}
                        />
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-4 col-md-12 from-group">
                        <label>Decision State(s)</label>
                        <CheckboxGroup
                            field={state.form.decisionStates}
                            options={decisionStateOptions}
                        />
                    </div>
                    <div class="col-lg-4 col-md-12 form-group">
                        <label>Filters</label>
                        <CheckboxGroup field={state.form.toggles} options={toggleOptions} />
                    </div>
                    <div class="col-lg-4 col-md-12 form-group align-self-end">
                        <button type="submit" class="btn btn-primary">
                            Search
                        </button>
                    </div>
                </div>
            </form>
        );
    }

    return { view, oninit };
}