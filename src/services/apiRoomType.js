import axios from 'axios';
import BASE_URL from '../utils/BASE_URL';

export default async function getRoomTypes() {
    try {
        const roomTypesRes = await axios.get(`${BASE_URL}/roomTypes`);
        
        return roomTypesRes.data; 
        
    } catch (error) {
        console.error("this is an error in fetch roomTypes", error.message);
        return []; 
    }
}