import m from 'mithril'

const noop = () => {};

export default function RadioGroup() {

    function view({
        attrs: {
            field,
            options = null,
            onchange = noop,
            class: classes = null,
            ...otherAttrs
        }
    }) {
        options = options || field.config.options || [];
        const name = field.fieldName;
        return (
            <div class="form-group">
                {options.map(({ label, value  }) => {
                    const id = `${name}_${label}`;
                    return (
                        <div class="custom-control custom-radio">
                            <input
                                class="custom-control-input"
                                type="radio"
                                id={id}
                                value={value}
                                name={name}
                                onclick={e => {
                                    const selected = e.target.checked;
                                    const option = new Option(label, value, false, selected);
                                    field.setData(option);
                                    onchange(option);
                                }}
                                {...otherAttrs}
                            />
                            <label class="custom-control-label" for={id}>
                                {label}
                            </label>
                        </div>
                    );
                })}
            </div>
        );
    }

    return { view }
}