import m from 'mithril'

import DataTable from 'Components/DataTable'
import {
    COLUMN_NAMES as DECISION_TABLE_COLUMN_NAMES,
    makeDecisionsTableConfig,
    default as DecisionsTable
} from 'Components/SearchPage/DecisionsTable.js'

window.CART = window.CART || {
    handlers: [],
    items: []
}

export const CART_EVENTS = {
    CART_CHANGED: 'CART_CHANGED'
};

export function makeCartTableConfig(cartItems) {
    const decisionsTableConfig = makeDecisionsTableConfig(cartItems);
    const columnsToKeep = [
        DECISION_TABLE_COLUMN_NAMES.Type,
        DECISION_TABLE_COLUMN_NAMES.ExchangeUniversity,
        DECISION_TABLE_COLUMN_NAMES.ExchangeUnit,
        DECISION_TABLE_COLUMN_NAMES.UWAUnit
    ];
    return {
        ...decisionsTableConfig,
        columns: [
            ...decisionsTableConfig.columns.filter(c => columnsToKeep.includes(c.title)),
            {
                title: 'Remove from Cart',
                data: null,
                defaultContent: "<button type='button' class='btn btn-primary'>🗑️</button>"
            }
        ]
    }
}

export function emitCartEvent(name, ...args) {
    if (name === CART_EVENTS.CART_CHANGED) {
        args = [window.CART.items, ...args]
    }

    for (let handler of window.CART.handlers) {
        if (handler.name === name) {
            handler.func(...args);
        }
    }

    m.redraw();
}

export function addItemToCart(item, emitEvent = true) {
    /* Avoid adding duplicates to a cart by removing any previous copies
     * of the item.
     */
    removeItemFromCart(item, false);
    window.CART.items = [item, ...window.CART.items];
    if (emitEvent) {
        emitCartEvent(CART_EVENTS.CART_CHANGED);
    }
}

export function removeItemFromCart(item, emitEvent = true) {
    window.CART.items = window.CART.items.filter(i => i !== item);
    if (emitEvent) {
        emitCartEvent(CART_EVENTS.CART_CHANGED);
    }
}

export function addCartEventHandler(name, callback) {
    window.CART.handlers.push({
        name,
        func: callback
    });
}

export function isItemInCart(item) {
    return window.CART.items.indexOf(item) >= 0;
}

export function removeCartEventHandler(name, callback) {
    const index = window.CART.handlers.items.find(
        h => h.name === name && h.callback === callback
    );
    if (index >= 0) {
        window.CART.handlers.splice(index, 1);
    }
}

export default function Cart() {

    function view() {
        return (
            <div>
                {window.CART.items.length === 0 ? <h6>Cart is empty.</h6> : <div />}
                <DataTable
                    config={makeCartTableConfig(window.CART.items)}
                    setup={(id, datatable) => {
                        $(`#${id} tbody`).on('click', 'button', function(event) {
                            const item = datatable.row($(event.target).parents('tr')).data();
                            removeItemFromCart(item);
                        })
                    }}
                />
            </div>
        );
    }

    return { view };
}