import { useMutation, useQueryClient } from "@tanstack/react-query";
import { addRoom as addRoomApi } from "../../services/apiRoom";
import toast from 'react-hot-toast';
import { ModalContext } from "../../ui/Modal";
import { useContext } from "react";


export default function useAddRoom() {
      const queryClient = useQueryClient();
  const { close } = useContext(ModalContext);

    const{mutate:addRoom,error ,isPending:isLoading}=useMutation({
        mutationFn:addRoomApi,
        mutationKey:['rooms']
        ,onSuccess:()=>{
            toast.success("room add successfully")
            queryClient.invalidateQueries({
                    queryKey: ["rooms"]
            });
            close();
        },
        onError:(err)=>{
            toast.error("can't add room",err)
            console.log(err)
        }
    })      

    return{addRoom ,error ,isLoading}
}
