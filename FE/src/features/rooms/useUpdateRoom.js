import { useMutation, useQueryClient } from "@tanstack/react-query";
import { updateRoom as updateRoomApi } from "../../services/apiRoom";
import toast from 'react-hot-toast';
import { ModalContext } from "../../ui/Modal";
import { useContext } from "react";

export default function useUpdateRoom() {
    const queryClient = useQueryClient();
  const { close } = useContext(ModalContext);

    const{mutate:updateRoom,error ,isPending:isLoading}=useMutation({
        mutationFn:({id,updatedData})=>updateRoomApi({id,updatedData}),

        onSuccess:()=>{
            toast.success("room updated successfully"),
            queryClient.invalidateQueries({queryKey: ["rooms"]});
            close()
        },
        onError:(err)=>{
            toast.error("can't updated room")
            console.log(err)
        }
    })      

    return{updateRoom ,error ,isLoading}
}
