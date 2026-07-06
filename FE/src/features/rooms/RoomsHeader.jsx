import { IoIosAddCircleOutline } from "react-icons/io";
import Modal from "../../ui/Modal";
import RoomCards from "./RoomCards";
import RoomsFilter from "./RoomsFilter";
import FormAddRoom from "./FormAddRoom";

export default function RoomsHeader() {
  return (
    <div className="m-2 flex justify-between  ">
    
      <div>
        <h1 className="font-bold text-xl text-[var(--primary)]">rooms management </h1>
      </div>
      
      <Modal>
        <Modal.Open opens='add-room'>
        <button className="bg-[var(--primary-hover)] text-[15px] text-[var(--text-secondary)]
        font-bold p-1 rounded-xl flex items-center gap-1 
        ">
          <span className="text-xl">< IoIosAddCircleOutline/> </span>  Add room
        </button>
        </Modal.Open>
        <Modal.Window name='add-room'>
        <FormAddRoom/>
        </Modal.Window>
    </Modal>
    </div>
  )
}
