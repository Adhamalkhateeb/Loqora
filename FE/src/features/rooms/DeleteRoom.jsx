import Modal from '../../ui/Modal'
import ConfirmDelete from '../../ui/ConfirmDelete'
import { MdDelete } from "react-icons/md";

export default function DeleteRoom({onConfirm ,disabled}) {
  return (
    <Modal>
      <Modal.Open opens='delete-room'>
        <button className="text-gray-400 hover:text-red-500 transition text-xl">
          <MdDelete />
        </button>
      </Modal.Open>
      <Modal.Window name='delete-room'>
        <ConfirmDelete resourceName='room' onConfirm={onConfirm} disabled={disabled}/>
      </Modal.Window>
    </Modal>
  ) 
}
