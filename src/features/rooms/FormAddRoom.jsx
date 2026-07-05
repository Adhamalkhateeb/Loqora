import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import toast from 'react-hot-toast';
import Form from '../../ui/Form';
import FormRow from '../../ui/FormRow';
import Input from '../../ui/Input';
import useRoomTypes from '../roomType/useRoomTypes';
import useAddRoom from './useAddRoom';
import useRooms from './useRooms';
import useUpdateRoom from './useUpdateRoom';
export default function FormAddRoom({item}) {
  const { roomTypes } = useRoomTypes();
  const { register, handleSubmit, formState: { errors } ,reset } = useForm({defaultValues:
 item
  ? {
      roomNumber: item.roomNumber,
      roomTypeId: item.roomTypeId,
      status: item.status,
      floor: item.floor,
    }
  : {
      roomNumber: "",
      roomTypeId: "",
      status: "",
      floor: "",
    }
  });

  useEffect(() => {
  if (item) {
    reset({
      roomNumber: item.roomNumber,
      roomTypeId: item.roomTypeId,
      status: item.status,
      floor: item.floor
    });
  }
}, [item, reset]);


  const {addRoom ,isLoading:isAdding}= useAddRoom()
  const{updateRoom ,isLoading:isUpdating}=useUpdateRoom()  
  const isWorking = isAdding || isUpdating
  const {rooms}=useRooms()  

function validateRoom(formData, rooms, currentId = null) {
  
  const isExist = rooms.some(
    (room) =>
      room.roomNumber === formData.roomNumber &&
      room.id !== currentId
  );

  if (isExist) {
    return "Room number already exists";
  }

  const roomNum = Number(formData.roomNumber);

  const min = Number(formData.floor) * 100 + 1;
  const max = Number(formData.floor) * 100 + 99;

  if (roomNum < min || roomNum > max) {
    return `Floor ${formData.floor} should have rooms between ${min}-${max}`;
  }

  return null;
}

const onSubmit = (data) => {
  const error = validateRoom(data, rooms,item?.id);

  if (error) {
    toast.error(error);
    return;
  }
  
  if(!item){
  addRoom(data)
  }else{
    updateRoom({
      id:item.id,
      updatedData:data
    })
  }
  
  ;
};


  return (
    <Form onSubmit={handleSubmit(onSubmit)}>
      
      <FormRow label={'room number'} error={errors.roomNumber?.message}>
        <Input 
          type="number" 
          name="roomNumber" 
          id="roomNumber"  
          placeholder='Enter room number' 
          min={1}
          register={register}
          required={true} 
        />
      </FormRow>   
      
      <FormRow label={'room type'} error={errors.roomTypeId?.message}>
        <select 
          id="roomTypeId" 
          className="w-full border border-gray-350 rounded-xl px-4 py-2 text-gray-700 bg-white focus:outline-none focus:border-[var(--secondary)] focus:ring-2 focus:ring-[var(--secondary)]/10 transition-all duration-200 cursor-pointer"
          {...register('roomTypeId', { required: "Please select a room type" })}
        >
          <option value="">Select Type...</option>
          {roomTypes.map((type) => (
            <option key={type.id} value={type.id}>
              {type.name}
            </option>
          ))}
        </select>
      </FormRow>   
      
<FormRow label={'status'} error={errors.status?.message}>
  <select 
    id="status" // تم تعديل الـ id ليكون معبراً عن الحقل
    className="w-full border border-gray-350 rounded-xl px-4 py-2 text-gray-700 bg-white focus:outline-none focus:border-[var(--secondary)] focus:ring-2 focus:ring-[var(--secondary)]/10 transition-all duration-200 cursor-pointer"
    {...register('status', { required: "Please select a room status" })} // تعديل رسالة الخطأ
  >
    <option value="">Select status...</option>
    
    <option value="available">Available</option>
    <option value="occupied">Occupied</option>
    <option value="reserved">Reserved</option>
    <option value="maintenance">Maintenance</option>
    <option value="cleaning">cleaning</option>
  </select>
</FormRow>

      <FormRow label={'floor'} error={errors.floor?.message}>
        <Input 
          type="number" 
          name="floor" 
          id="floor"  
          placeholder='Enter floor number' 
          min={1}
          register={register}
          required={true}
        />
      </FormRow>   
      
      <FormRow label={'notes'} error={errors.notes?.message}>
        <Input 
          type="text" 
          name="notes" 
          id="notes"  
          placeholder='Add any additional notes'
          register={register}
          required={false}
        />
      </FormRow>   

      <div className="flex justify-end mt-2">
        <button 
          type="submit"
   
          disabled={isWorking}
          className='w-full sm:w-auto justify-center items-center bg-[var(--secondary)] hover:opacity-90 py-2.5 px-6 text-lg text-[var(--white-)] font-medium rounded-xl transition-all duration-200 shadow-sm'
        >
         { item ?"update room":"Add Room"}
        </button>
      </div>
    </Form>
  )
}