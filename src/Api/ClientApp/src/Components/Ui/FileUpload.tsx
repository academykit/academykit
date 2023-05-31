import { useState } from "react";
import { FilePond, registerPlugin } from "react-filepond";
import "filepond/dist/filepond.min.css";
import FilePondPluginImageExifOrientation from "filepond-plugin-image-exif-orientation";
import FilePondPluginImagePreview from "filepond-plugin-image-preview";
import FilePondPluginImageValidateSize from "filepond-plugin-image-validate-size";
import FilePondPluginFileValidateType from "filepond-plugin-file-validate-type";
import "filepond-plugin-image-preview/dist/filepond-plugin-image-preview.css";
import { useAddGroupAttachment } from "@utils/services/groupService";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import { useTranslation } from "react-i18next";
registerPlugin(
  FilePondPluginImageExifOrientation,
  FilePondPluginImagePreview,
  FilePondPluginImageValidateSize,
  FilePondPluginFileValidateType
);
type IProps = {
  label?: string;
  FormField?: string;
  currentThumbnail?: string;
  id: string;
  onSuccess: () => void;
  search: string;
};

const FileUpload = ({ label = "files", onSuccess, id, search }: IProps) => {
  const [files, setFiles] = useState<any>([]);
  const postAttachment = useAddGroupAttachment(search);

  const { t } = useTranslation();
  return (
    <div style={{ maxWidth: 470, position: "relative" }}>
      <FilePond
        instantUpload={true}
        files={files}
        labelIdle={`Drag & Drop your ${label} or <span class="filepond--label-action">Browse</span>`}
        onaddfile={(error, file) => {}}
        onupdatefiles={setFiles}
        allowMultiple={false}
        maxFiles={1}
        credits={false}
        server={{
          remove: null,
          revert: null,
          //processing a file
          process: async (
            fieldName,
            file,
            metadata,
            load,
            error,
            progress,
            abort
          ) => {
            try {
              const res = await postAttachment.mutateAsync({
                groupIdentity: id as string,
                file: file as File,
              });
              load(res.data.url);
              showNotification({ message: t("add_attachment_success") });
              onSuccess();
            } catch (err) {
              const resError = errorType(err);
              error(resError);
              showNotification({
                message: resError,
                color: "red",
              });
            }
          },
          load: async (source, load, error, progress, abort, headers) => {
            await fetch(
              `${source}?cache=${Math.random().toString(36).substring(2, 7)}`
            )
              .then(async (r) => {
                load(await r.blob());
              })
              .catch((r) => error(r));
            // Should expose an abort method so the request can be cancelled
            return {
              abort: () => {
                abort();
              },
            };
          },
        }}
      />
    </div>
  );
};

export default FileUpload;
