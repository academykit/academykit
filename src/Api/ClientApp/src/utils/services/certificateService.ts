import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "./service-api";
import { httpClient } from "./service-axios";
import { IUser } from "./types";

export interface ExternalCertificatePost{
    name: string;
    startDate: string;
    endDate: string;
    imageUrl: string;
    location: string;
    institute: string;
    duration: number
  }

export interface GetExternalCertificate extends ExternalCertificatePost
    {
      id:string;
      
      isVerified: boolean;
      user: IUser
    }
  

const getExternalCertificate = () => {
    return httpClient.get<GetExternalCertificate>(api.externalCertificate.add)
}

export const useGetExternalCertificate = () => {
    useQuery(['certificate',api.externalCertificate.add],
    
    () => getExternalCertificate(), {
        select: data => data.data
    })
}


  const addCertificate = (data: ExternalCertificatePost) => {
    return httpClient.post(api.externalCertificate.add,data)
  }

  export const useAddCertificate =()=> {
    const queryClient = useQueryClient();
    return useMutation(["post"+api.externalCertificate.add],addCertificate, {
        onSuccess : () => {
queryClient.invalidateQueries(['certificate',api.externalCertificate.add])
        }
    })
  }