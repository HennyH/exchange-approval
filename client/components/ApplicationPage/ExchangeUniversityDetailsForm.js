import m from 'mithril'
import { Form } from 'Components/FormHelpers'

import { Input, StringField } from 'FormHelpers'

export class ExchangeUniversityDetailsPowerForm extends Form {
    configureFields() {
        this.universityName = StringField.new({ required: true });
        this.universityHomepage = StringField.new({
            required: true,
            regex: /(^https?\:\/\/)|(^www.)/,
            regexErrorMessage: 'Enter a URL of the form https:// or http://...'
        });
        this.universityCountry = StringField.new({ required: true });
    }
}

export function ExchangeUniversityDetailsForm() {

    function view({ attrs: { form, staffView } }) {

        function oninit(vnode) {
            vnode.staffView
        }

        return (
            <form novalidate>
                <div class="form-group row mx-1">
                    <label class="col-form-label col-3" for="email">Exchange University Country: </label>
                    <div class="input-group col-8">
                        <Input field={form.universityCountry} placeholder="University Country" type="text" readonly={staffView} />
                    </div>
                </div>
                <div class="form-group row mx-1">
                    <label class="col-form-label col-3" for="email">Exchange University Name: </label>
                    <div class="input-group col-8">
                        <Input field={form.universityName} placeholder="University Name" type="text" readonly={staffView} />
                    </div>
                </div>
                <div class="form-group row mx-1">
                    <label class="col-form-label col-3" for="email">Exchange University Homepage: </label>
                    <div class="input-group col-8">
                        <Input field={form.universityHomepage} placeholder="University Website" type="text" readonly={staffView} />
                    </div>
                </div>
            </form>
        )
    }

    return { view }
}