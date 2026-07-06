import Modal from '../../ui/Modal'
import ConfirmDelete from '../../ui/ConfirmDelete'
import { CiEdit } from 'react-icons/ci'
import FormAddRoom from './FormAddRoom'

export default function UpdateRoom({item}) {
  return (
    <Modal>
      <Modal.Open opens='edit-room'>
          <button className=" text-gray-400   hover:text-blue-500 transition text-xl">
            <CiEdit />
          </button>
      </Modal.Open>
      <Modal.Window name='edit-room'>
        <FormAddRoom item={item}/>
      </Modal.Window>
    </Modal>
  )
}
