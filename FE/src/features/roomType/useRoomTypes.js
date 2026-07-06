import { useQuery } from '@tanstack/react-query'
import getRoomTypes from '../../services/apiRoomType'
export default function useRoomTypes() {
    const {  data: roomTypes = [] , error , isPending:isLoading}=useQuery({
        queryFn:getRoomTypes,
        queryKey:["roomTypes"],   
        staleTime: 1000 * 60 * 5,
    }
)
return {roomTypes , error , isLoading}
}
