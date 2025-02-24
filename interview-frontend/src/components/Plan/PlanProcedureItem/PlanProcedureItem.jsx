import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";
import { addUserToProcedure, deleteUsersFromProcedure } from "../../../api/api";

const PlanProcedureItem = ({ procedure, users,planId }) => {
    const [selectedUsers, setSelectedUsers] = useState([]);

    useEffect(() => {
        if (procedure && procedure.planProcedureUsers && procedure.planProcedureUsers.length > 0) {
            var ppUsers = procedure.planProcedureUsers.filter(ppu => ppu.procedureId === procedure.procedureId)
            if (ppUsers && ppUsers.length > 0) {
                const pUsers = ppUsers
                .map(ppu => users.find(u => u.value === ppu.userId))
                .filter(Boolean); 
                setSelectedUsers(pUsers);
            }
        }
    }, [planId, procedure, users]);

    const handleAssignUserToProcedure = async(e) => {
        setSelectedUsers(e);
        var userIds = e.map((item)=> item.value);
        if (userIds.length > 0)
            await addUserToProcedure(planId, procedure.procedureId, userIds);
        else
            await deleteUsersFromProcedure(planId, procedure.procedureId, userIds);
    };

    return (
        <div className="py-2">
            <div>
                {procedure.procedureTitle}
            </div>

            <ReactSelect
                className="mt-2"
                placeholder="Select User to Assign"
                isMulti={true}
                options={users}
                value={selectedUsers}
                onChange={(e) => handleAssignUserToProcedure(e)}
            />
        </div>
    );
};

export default PlanProcedureItem;
