import m from 'mithril';
import classNames from 'classnames';

import Layout from 'Components/Layout';
import { addItemToCart, default as Cart } from 'Components/Cart';
import DecisionSearchSettingsPanel, { DecisionSearchSettingsPowerForm } from './DecisionSearchSettingsPanel.js';
import DecisionsTable from './DecisionsTable.js';
import PersistentDataLoader from 'Components/PersistentDataLoader.js'
import Spinner from 'Components/Spinners/RectangularSpinner.js';

window.SEARCH_SETTINGS_FORM = window.SEARCH_SETTINGS_FORM ||  null;


function UnitSearch() {

    const state = {
        searchSettings: { universityNames: [], uwaUnitLevels: [] }
    }

    function handleSearchSettingsChanged(settings) {
        const universityNames = settings.exchangeUniversities.map(({ value }) => value);
        const uwaUnitLevels = settings.unitLevels.map(({ value }) => value);
        state.searchSettings = { universityNames, uwaUnitLevels };
        m.redraw();
    }

    function fetchEquivalencies(settings) {
        const qs = m.buildQueryString(settings);
        return m.request({ method: "GET", url: `/api/equivalencies?${qs}` });
    }

    function view() {
        return(
            <div class="card mt-3">
                <div class="card-header">
                    Unit Search
                </div>
                <div class="card bg-light m-3">
                <div class="card-header">Search Settings</div>
                <PersistentDataLoader
                    id="search-page-filters"
                    requests={{filters: () => m.request("/api/filters/student")}}
                    render={({loading, errored, data: { filters: { exchangeUniversityNameOptions, uwaUnitLevelOptions } = {} }}) => {
                        const hideForm = !!(loading || errored)
                        if (!hideForm && window.SEARCH_SETTINGS_FORM === null) {
                            window.SEARCH_SETTINGS_FORM = new DecisionSearchSettingsPowerForm({
                                exchangeUniversityNameOptions,
                                uwaUnitLevelOptions
                            })
                        }
                        return (
                            <div class={classNames("card-body", loading ? "text-center" : "")}>
                                {hideForm
                                    ? <Spinner />
                                    : (
                                        <DecisionSearchSettingsPanel
                                            form={window.SEARCH_SETTINGS_FORM}
                                            onSearchSettingsChanged={handleSearchSettingsChanged}
                                        />
                                    )}
                            </div>
                        );
                    }}
                />
                </div>
                <div class="card bg-light m-3">
                    <div class="card-header">Search Results</div>
                        <PersistentDataLoader
                            id="search-page-equivalencies"
                            settings={state.searchSettings}
                            requests={{equivalencies: ({settings}) => fetchEquivalencies(settings)}}
                            render={({ loading, errored, data: { equivalencies } = {}}) => (
                                <div class={classNames("card-body", loading ? "text-center" : "")}>
                                    {(loading
                                        ? <Spinner />
                                        : <DecisionsTable decisions={equivalencies} onAddToCart={addItemToCart} />)}
                                </div>
                            )}
                        />
                </div>
            </div>
        )
    }

    return { view };
}

export default function SearchPage() {

    function view() {

        return (
            <Layout>
                <div class="container-fluid">
                    <UnitSearch/>
                </div>
                <div class="container-fluid">
                    <Cart/>
                </div>
            </Layout>
        );
    }

    return { view };
}
