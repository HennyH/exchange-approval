import m from 'mithril'

import DataTable from 'Components/DataTable/DataTable.js';
import { ModalState } from '../ViewData';
import DataLoader from 'Components/DataLoader.js'
import ApplicationForm, { ApplicationPowerForm } from 'Components/ApplicationPage/ApplicationForm.js'
import Spinner from 'Components/Spinners/RectangularSpinner.js';
import Modal from '../Modal/Modal.js';
import { EmailData } from '../ViewData.js';
import Styles from './InboxTable.css';

const noop = () => {};

export const COLUMN_NAMES = {
    Id: 'Id',
    StudentName: "Student Name",
    StudentNumber: "Student Number",
    Date: 'Date',
    Type: 'Type',
    ExchangeUniversityName: 'Ex. University',
    Approved: 'Appv.',
    Edit: 'Edit'
};

export function makeInboxTableConfig(decisions) {
    return {
        data: decisions,
        columns: [
            {
                title: COLUMN_NAMES.Date,
                data: "lastUpdatedAt",
                render: (data, type, row, meta) => {
                    if (data === null) {
                        return `
                            <span class='badge badge-warning'>
                                PENDING
                            </span>
                        `;
                    }
                    const options = { day: "numeric", month: "short", year: "numeric" };
                    return new Date(data).toLocaleDateString(undefined, options);
                }
            },
            {
                title: COLUMN_NAMES.StudentName,
                data: "studentName"
            },
            {
                title: COLUMN_NAMES.StudentNumber,
                data: "studentNumber"
            },
            {
                title: COLUMN_NAMES.ExchangeUniversityName,
                data: "exchangeUniversityName",
                width: '30%',
                render: (data, type, row, meta) =>
                `<a href=${encodeURI(row.exchangeUniversityHref)} target="_blank">${data}</a>`
            },
            {
                title: COLUMN_NAMES.Approved,
                data: "studentApplicationStatus",
                render: (data, type, row, meta) => data.label
            },
            {
                title: COLUMN_NAMES.Edit,
                render: (data, type, row, meta) =>
                    `<button type='button' class='btn btn-primary'>🖉 Edit</button>`
            }
        ]
    }
}

function StaffEditApplicationModal() {

    const state = {
        applicationForm: null,
        modalRef: null
    };

    function fetchApplication(applicationId) {
        const qs = m.buildQueryString({ id: applicationId });
        return m.request(`/api/application?${qs}`);
    }

    function submitAppliction(onSubmit, onClose) {
        state.submittingApplication = true;
        m.request({
            method: "POST",
            url: "/api/application/update",
            data: state.applicationForm.getData()
        })
        .then(async () => {
            if (state.modalRef) {
                state.modalRef.modal('hide');
            }
            await onSubmit();
            return onClose();
        });

    }

    function view({ attrs: { applicationId, onSubmit = noop, onClose = noop } }) {
        return (
            <DataLoader
                applicationId={applicationId}
                requests={{
                    application: ({ applicationId }) => fetchApplication(applicationId),
                    filters: () => m.request("/api/filters/staff")
                }}
                render={({ loading, errored, data: { application, filters } = {} }) => {
                    const showLoading = !!(loading || errored);
                    if (state.applicationForm === null && !showLoading) {
                        state.applicationForm = new ApplicationPowerForm({
                            unitLevelOptions: filters.uwaUnitLevelOptions,
                            studentOfficeOptions: filters.studentOfficeOptions,
                        });
                        state.applicationForm.setData(application);
                        state.applicationForm.setDirty(false);
                    }
                    const modalFooter = ([
                        <div>
                            <button disabled={showLoading} type="button" class="btn btn-outline-primary mx-1" onclick={() => EmailData.Student.SendEmail()}>
                                ✉ Send Application Results
                            </button>
                            <button disabled={showLoading} type="button" class="btn btn-outline-secondary mx-1" onclick={() => EmailData.Student.CopyText()}>
                                📋 Copy to Clipboard
                            </button>
                        </div>,
                        <div>
                            <button type="button" class="btn btn-secondary mx-1" data-dismiss="modal">
                                Cancel
                            </button>
                            <button disabled={showLoading} type="button" class="btn btn-primary mx-1" onclick={() => submitAppliction(onSubmit, onClose)}>
                                Save Application
                            </button>
                        </div>
                    ]);
                    return applicationId && (
                        <Modal
                            size="xl"
                            title="Edit Application"
                            footer={modalFooter}
                            onClose={() => {
                                state.applicationForm = null;
                                onClose();
                            }}
                            ref={(ref) => {
                                state.modalRef = ref;
                            }}
                        >
                            {(showLoading
                                ? (
                                    <div class="text-center pt-3 pb-3">
                                        <Spinner />
                                    </div>
                                )
                                : <ApplicationForm form={state.applicationForm} staffView={true} />
                            )}
                        </Modal>
                    );
                }}
            />
        );
    }

    return { view };
}

window.INBOX_DATATABLE = window.INBOX_DATATABLE || null;
window.INBOX_SELECED_APPLICATION_ID = window.INBOX_SELECED_APPLICATION_ID || null;
window.INBOX_SELECED_ROW = window.INBOX_SELECED_ROW || null;

export default function InboxTable() {

    function view({ attrs: { data }}) {
        return (
            <div>
                <DataTable
                    id="inbox-table"
                    config={makeInboxTableConfig(data)}
                    setup={($ref, datatable) => {
                        window.INBOX_DATATABLE = datatable;
                        $ref.on('click', 'button', (event) => {
                            const row = $(event.target).parents('tr');
                            window.INBOX_SELECED_ROW = row;
                            window.INBOX_SELECED_APPLICATION_ID = datatable.row(row).data().studentApplicationId;
                            m.redraw();
                        });
                    }}
                    cache={true}
                />
                {window.INBOX_SELECED_APPLICATION_ID && (
                    <StaffEditApplicationModal
                        applicationId={window.INBOX_SELECED_APPLICATION_ID}
                        onSubmit={() => {
                            const qs = m.buildQueryString({
                                applicationId: window.INBOX_SELECED_APPLICATION_ID
                            });
                            m.request(`/api/inbox/application?${qs}`).then((inboxItem) => {
                                if (window.INBOX_SELECED_ROW) {
                                    window.INBOX_DATATABLE.row(window.INBOX_SELECED_ROW).data(inboxItem).invalidate();
                                    window.INBOX_SELECED_ROW.addClass(Styles.blink);
                                    setTimeout(() => {
                                        window.INBOX_SELECED_ROW.removeClass(Styles.blink);
                                    }, 1000);
                                }
                            });
                        }}
                        onClose={() => {
                            window.INBOX_SELECED_APPLICATION_ID = null;
                            window.INBOX_SELECED_ROW = null;
                        }}
                    />
                )}
            </div>
        )
    }

    return { view }
}