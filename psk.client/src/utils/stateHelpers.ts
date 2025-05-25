export const mergeEntities = <T extends { id: string }>(
    currentItems: T[],
    newItem: T
): T[] => {
    // Check if item already exists
    const index = currentItems.findIndex(item => item.id === newItem.id);

    if (index === -1) {
        // Item doesn't exist, add it
        return [...currentItems, newItem];
    } else {
        // Item exists, update it
        return currentItems.map(item =>
            item.id === newItem.id ? newItem : item
        );
    }
};