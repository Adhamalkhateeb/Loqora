import axios from "axios";
import BASE_URL from "../utils/BASE_URL";

export default async function getAmenities() {
    const roomTypesRes = await axios.get(`${BASE_URL}/a`)
    const roomType = roomTypesRes.data
    return roomType
}