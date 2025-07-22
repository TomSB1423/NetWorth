export const getInstitutions = async () => {
    try {
        const response = await fetch(`api/institutions`, {
            method: 'GET',
            headers: {
                'accept': 'application/json',
            },
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error fetching institutions:', error);
        throw error;
    }
};
