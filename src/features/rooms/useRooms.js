import { useQuery } from "@tanstack/react-query";
import getRoomsWithDetails from "../../services/apiRoom";

export default function useRooms() {
  const {
    data: rooms = [],
    error,
    isPending: isLoading
  } = useQuery({
    queryFn: getRoomsWithDetails,
    queryKey: ["rooms"],
    
  staleTime: 1000 * 60 * 5, 

  gcTime: 1000 * 60 * 60,
    placeholderData: (previousData) => previousData
  });

  return { rooms, error, isLoading };
}