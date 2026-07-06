import BASE_URL from "../utils/BASE_URL.js";
import axios from "axios";

export default async function getRoomsWithDetails() {
    console.log("FETCHING AGAIN");
  try {
    const [
      roomsRes,
      roomTypesRes,
      imagesRes,
      amenitiesRes,
      roomTypeAmenitiesRes
    ] = await Promise.all([
      axios.get(`${BASE_URL}/rooms`),
      axios.get(`${BASE_URL}/roomTypes`),
      axios.get(`${BASE_URL}/roomTypeImages`),
      axios.get(`${BASE_URL}/amenities`),
      axios.get(`${BASE_URL}/roomTypeAmenities`)
    ]);

    const rooms = roomsRes.data;
    const roomTypes = roomTypesRes.data;
    const roomTypeImages = imagesRes.data;
    const amenities = amenitiesRes.data;
    const roomTypeAmenities = roomTypeAmenitiesRes.data;

    const fullRooms = rooms.map((room) => {

      // get room type
      const roomType = roomTypes.find(
        (type) => Number(type.id) === Number(room.roomTypeId)
      );

      // get images
      const images = roomTypeImages.filter(
        (image) => Number(image.roomTypeId) === Number(room.roomTypeId)
      );

      // get relations
      const relations = roomTypeAmenities.filter(
        (relation) => Number(relation.roomTypeId) === Number(room.roomTypeId)
      );

      // get amenity ids
      const amenityIds = relations.map(
        (relation) => relation.amenityId
      );

      // get actual amenities
      const roomAmenities = amenities.filter(
        (amenity) => amenityIds.includes(amenity.id)
      );

      return {
        ...room,

        roomType,

        images,

        amenities: roomAmenities
      };
    });

    return fullRooms;

  } catch (error) {
    console.error(error);
    return [];
  }
  console.log("FETCH DONE");
}

export async function addRoom(data) {
  try {
    const response = await axios.post(
      `${BASE_URL}/rooms`,
      data
    );

    return response.data;
  } catch (error) {
    console.error(
      "this is an error in add room",
      error.message
    );

    throw new Error(error.message);
  }
}
export async function deleteRoom(id) {
  try {

    const response = await axios.delete(
      `${BASE_URL}/rooms/${id}`
    );

    return response.data;

  } catch (error) {

    console.error(error.message);

    throw new Error(error.message);
  }
}
export async function updateRoom({ id, updatedData }) {
  try {
    const response = await axios.patch(
      `${BASE_URL}/rooms/${id}`,
      updatedData
    );

    return response.data;
  } catch (error) {
    console.error(error.message);
    throw new Error(error.message);
  }
}