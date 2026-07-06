import FilterBtns from "../../ui/FilterBtns";
import Search from "../../ui/Search";
import useRooms from "./useRooms";

export default function RoomsFilter({rooms}) {

    const roomTypes = [ ...new Set(rooms.map((room) => room.roomType?.category))];
    
    const roomTypeOptions = [
    { value: "all", label: "All types" },

    ...roomTypes.map((type) => ({
        value: type,
        label: type
    }))
    ]
    const roomStatus = [ ...new Set(rooms.map((room) => room.status))]
    const roomStatusOperation = [
    { value:"all", label:"All status" },
    ...roomStatus.map((type)=>(
    {
        value:type, 
        label:type
    }))
    ]
    

  return (
    <div className="bg-[var(--white-)] p-4 rounded-xl flex justify-center items-center flex-col gap-4 lg:flex-row ">

    <Search text='search rooms by number or type...' />
        <div className="flex flex-wrap gap-4 items-center justify-center lg:flex-nowrap">
            <FilterBtns
            filterField="type"
            options={roomTypeOptions}
            />
            <FilterBtns
            filterField="status"
            options={roomStatusOperation}
            />
            <FilterBtns
            filterField="sortBy"
            options={[
                { value:"default", label:"Sort default" },
                { value:"asc", label:"Price low to high" },
                { value:"desc", label:"Price high to low" }
            ]}
            />

        </div>

    </div>
  )
}
