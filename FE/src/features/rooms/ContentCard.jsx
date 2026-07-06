import { MdDelete } from "react-icons/md";
import { CiEdit } from "react-icons/ci";
import DeleteRoom from "./DeleteRoom";
import useDeleteRoom from "./useDeleteRoom";
import UpdateRoom from "./UpdateRoom";

export default function ContentCard({ item }) {
  const{deleteRoom,isLoading}=useDeleteRoom()

  
  return (
    <div className="p-4">

      <div className="flex justify-between items-start">

        <h2 className="font-semibold text-lg">
          Room {item.roomNumber}
        </h2>

        <span className="text-xs text-gray-500">
          Floor {item.floor}
        </span>

      </div>

      <p className="text-sm text-gray-500 mt-1">
        {item.roomType?.name}
      </p>

      <p className="text-xs text-gray-400 mt-2 line-clamp-2">
        {item.roomType?.description}
      </p>

      {/* Amenities */}

      <div className="flex gap-2 flex-wrap mt-3">

        {item.amenities?.slice(0, 3).map((amenity) => (
          <span
            key={amenity.id}
            className="
            text-xs
            bg-gray-100
            px-2
            py-1
            rounded-md
          "
          >
            {amenity.name}
          </span>
        ))}

      </div>

      <div className="mt-4 flex justify-between items-center">

        <span className="text-sm font-medium text-gray-600">
          {item.roomType?.capacityAdults} Adults
        </span>

        <div className="flex gap-3">
          <DeleteRoom onConfirm={()=>deleteRoom(item.id)} disabled={isLoading}/>
            <UpdateRoom item={item}/>
        </div>

      </div>

    </div>
  );
}