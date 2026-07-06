import { useMutation, useQueryClient } from "@tanstack/react-query";
import { deleteRoom as deleteRoomApi } from "../../services/apiRoom";
import toast from 'react-hot-toast';
import { ModalContext } from "../../ui/Modal";
import { useContext } from "react";

export default function useDeleteRoom() {
      const queryClient = useQueryClient();
  const { close } = useContext(ModalContext);

    const{mutate:deleteRoom ,error , isPending:isLoading}=useMutation({
        mutationFn:(id)=>deleteRoomApi(id),
        mutationKey:['rooms' ]       ,
        onSuccess:()=>{
            toast.success("room delete successfully"),
             queryClient.invalidateQueries({queryKey:["rooms"]});
             close()
            },
        onError:(err)=>{
            toast.error("can't delete room",err.message)
            console.log(err.message)
        }
    })
    return{deleteRoom ,isLoading,error}
}
