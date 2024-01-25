type Condition = [string, string, any];
type Data = Record<string, any>;

export function getFieldData(data: Data, field: string): any {
    if (!data) {
        return false;
    }
    const fieldKeys = field.split('.');
    let fieldValue = data;

    for (const key of fieldKeys) {
        fieldValue = fieldValue[key];
        if (fieldValue === undefined) {
            // Handle undefined fields
            return false;
        }
    }
    return fieldValue || false;
}


function evaluateCondition(data: Data, condition: Condition): boolean {
    const [field, operator, value] = condition;
    const fieldValue = getFieldData(data, field);
    switch (operator) {
        case '==':
            return fieldValue === value;
        case '=':
            return fieldValue === value;
        case '!=':
            return fieldValue !== value;
        case 'in':
            let curValue = value;
            if (!Array.isArray(value)) {
                curValue = [value]
            }
            return curValue.includes(fieldValue);
        // Add more operators as needed

        default:
            throw new Error(`Field: ${field},Unsupported operator: ${operator}`);
    }
}

function evaluateNestedConditions(data: Data, conditions: Condition[], logicalOperator: 'and' | 'or' = 'and'): boolean {
    for (const condition of conditions) {
        if (Array.isArray(condition)) {
            const result = evaluateCondition(data, condition);

            if (logicalOperator === 'and' && !result) {
                return false; // Short-circuit if any 'and' condition is false
            } else if (logicalOperator === 'or' && result) {
                return true; // Short-circuit if any 'or' condition is true
            }
        } else {
            throw new Error(`Invalid condition format: ${condition}`);
        }
    }

    return logicalOperator === 'and'; // All 'and' conditions must be true, or for 'or' at least one must be true
}



export function applyFilter(data: Data, filter: Condition | Condition[]): boolean {
    if (Array.isArray(filter)) {
        let andConditions: Condition[] = [];
        let orConditions: Condition[] = [];
        let currentLogicalOperator: 'and' | 'or' = 'and';

        for (const item of filter) {
            if (Array.isArray(item)) {
                if (currentLogicalOperator === 'and') {
                    andConditions.push(item as Condition);
                } else {
                    orConditions.push(item as Condition);
                }
            } else if (item === 'or') {
                currentLogicalOperator = 'or';
            } else if (item === 'and') {
                currentLogicalOperator = 'and';
            }
        }

        // Evaluate 'and' conditions first, then check 'or' conditions
        return evaluateNestedConditions(data, andConditions, 'and') || evaluateNestedConditions(data, orConditions, 'or');
    } else {
        return evaluateCondition(data, filter);
    }
}