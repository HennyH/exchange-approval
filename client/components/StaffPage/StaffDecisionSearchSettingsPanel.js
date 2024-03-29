import m from 'mithril';
import classNames from 'classnames';
import { Form } from 'Components/FormHelpers'

import { CheckboxGroup, Select2, OptionsField } from 'FormHelpers'
import Styles from './StaffDecisionSearchSettingsPanel.css';

export class StaffUnitSearchSettingsPowerForm extends Form {
    configureFields({
        studentOptions,
        studentOfficeOptions,
        applicationStateOptions
    }) {
        this.studentNumbers = OptionsField.new({
            multiple: true,
            options: studentOptions
        });
        this.studentOffices = OptionsField.new({
            multiple: true,
            options: studentOfficeOptions
        });
        this.applicationStatuses = OptionsField.new({
            multiple: true,
            options: applicationStateOptions
        });
    }
}


export default function StaffDecisionSearchSettingsPanel() {

    function handleSubmit(event, form, onSubmit) {
        event.preventDefault();
        event.stopPropagation();
        onSubmit(form.getData());
    }

    function view({ attrs: { form, onSubmit } }) {
        return (
            <form onsubmit={e => handleSubmit(e, form, onSubmit)}>
                <div class="row">
                    <div class="col-lg-4 col-md-12 form-group">
                        <label for="students">Students</label>
                        <Select2
                            field={form.studentNumbers}
                            config={{
                                multiple: true,
                                width: '100%',
                                placeholder: 'Select students to filter to...'
                            }}
                        />
                    </div>
                    <div class="col-lg-4 col-md-12 form-group pt-md-0 pt-sm-3 pt-3">
                        <label>Student Office</label>
                        <Select2
                            field={form.studentOffices}
                            config={{
                                multiple: true,
                                width: '100%',
                                placeholder: 'Select student offices to filter to...'
                            }}
                        />
                    </div>
                    <div class="col-lg-4 col-md-12 form-group pt-md-0 pt-sm-3 pt-3">
                        <label>Application Status</label>
                        <Select2
                            field={form.applicationStatuses}
                            config={{
                                multiple: true,
                                width: '100%',
                                placeholder: 'Select statuses to filter to...'
                            }}
                        />
                    </div>
                    <div class="col-lg-12 col-md-12 form-group">
                        <button type="submit" class="btn btn-primary" style="display: block; float: right;">
                            Search
                        </button>
                    </div>
                </div>
            </form>
        );
    }

    return { view };
}