import React, { useEffect, useState } from "react";
import ReactSelect from "react-select";
import { addUserToProcedure, deleteUsersFromProcedure,unassignUserFromPlanProcedure } from "../../../api/api";

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

    const handleAssignUserToProcedure = async (e) => {
        const currentUserIds = new Set(selectedUsers.map((u) => u.value));
        const newUserIds = new Set(e.map((u) => u.value));

        const usersToAssign = [...newUserIds].filter(
            (userId) => !currentUserIds.has(userId)
        );
        const usersToUnassign = [...currentUserIds].filter(
            (userId) => !newUserIds.has(userId)
        );

        // Add new users
        if (usersToAssign.length > 0) {
            await addUserToProcedure(planId, procedure.procedureId, usersToAssign);
        }

        // Unassign individual users 
        if (usersToUnassign.length === 1 && e.length!==0) {
            await unassignUserFromPlanProcedure(planId, procedure.procedureId, usersToUnassign[0]);
        }

        // Delete all users if none are selected
        if (e.length === 0) {
            await deleteUsersFromProcedure(planId, procedure.procedureId);
        }

        setSelectedUsers(e);
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
