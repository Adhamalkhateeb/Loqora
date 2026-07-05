import { FaCheckCircle } from "react-icons/fa";
import { MdHotel } from "react-icons/md";
import { IoTime } from "react-icons/io5";
import { BsFillBrushFill } from "react-icons/bs";
import { FaTools } from "react-icons/fa";
import Card from "./Card";
import React from "react";


const statusConfig = {
  available: {
    color: "bg-green-500",
    icon: <FaCheckCircle />,
    text: "Available",
  },

  occupied: {
    color: "bg-red-500",
    icon: <MdHotel />,
    text: "Occupied",
  },

  reserved: {
    color: "bg-yellow-500",
    icon: <IoTime />,
    text: "Reserved",
  },

  cleaning: {
    color: "bg-blue-500",
    icon: <BsFillBrushFill />,
    text: "Cleaning",
  },

  maintenance: {
    color: "bg-gray-600",
    icon: <FaTools />,
    text: "Maintenance",
  },
};
 function RoomCards({ rooms }) {
  return (
    <div className=" grid  grid-cols-1  min-[500px]:grid-cols-2 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5 p-4">

      {rooms.map((item) => {

        const primaryImage =
          item.images?.find((img) => img.isPrimary)?.imageUrl;
          const status = statusConfig[item.status?.toLowerCase()];
        return (
        <Card item={item} primaryImage={primaryImage} status={status}  key={item.id}/>
        );
      })}
    </div>
  );
}
export default React.memo(RoomCards);
