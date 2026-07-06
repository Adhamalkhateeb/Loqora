import React, { useState } from 'react'
import useRooms from '../features/rooms/useRooms'
import RoomsFilter from '../features/rooms/RoomsFilter'
import RoomsHeader from '../features/rooms/RoomsHeader'
import RoomCards from '../features/rooms/RoomCards'
import { useSearchParams } from "react-router-dom";
import useRoomTypes from '../features/roomType/useRoomTypes'
export default function Rooms() {
  const [searchParams]=useSearchParams()
  const filterType = searchParams.get("type") || "all";
  const filterStatus = searchParams.get("status") || "all";
  const sortBy = searchParams.get("sortBy") || "default";
  const {rooms=[]} = useRooms()
  const {roomTypes}= useRoomTypes()
  console.log(roomTypes)
  const filteredRooms = rooms?.filter((room)=>{
    if(
      filterType !=='all'&&
     room.roomType.category !== filterType
    ){
      return false
    }
    if(
      filterStatus !=='all' &&
      room.status !== filterStatus
    ){
      return false
    }
    return true
  })
   ?.sort((a, b) => {

    if (sortBy === "asc") {
      return a.roomType.basePrice - b.roomType.basePrice;
    }

    if (sortBy === "desc") {
      return b.roomType.basePrice - a.roomType.basePrice;
    }

    return 0;
  });


  return (
    <div className=' flex flex-col gap-10 '>
    <div className='flex flex-col gap-7'>
      <RoomsHeader/>
      <RoomsFilter rooms={rooms}/>
    </div>
    <RoomCards rooms={filteredRooms}/>

    </div>
  )
}
