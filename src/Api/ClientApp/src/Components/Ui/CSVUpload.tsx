import React, { useState } from "react";
import { FilePond } from "react-filepond";
import { Text } from "@mantine/core";

const CSVUpload = () => {
  const [files, setFiles] = useState<any>([]);
  return (
    <div>
      <FilePond
        // @ts-ignore

        credits={false}
        allowProcess={false}
        instantUpload={false}
        allowFileTypeValidation={true}
        allowMultiple={false}
        files={files}
        onupdatefiles={setFiles}
        acceptedFileTypes={[
          "text/csv",
          "application/vnd.openxmlformats-officedocument.presentationml.presentation",
          "application/vnd.ms-excel",
          "application/csv",
        ]}
        // onChange={handleFileUpload}
        labelIdle={`Drag & Drop your csv or <span class="filepond--label-action">Browse</span>`}
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
              abort();
            } catch (error) {}
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
      ></FilePond>
      <Text mt={1} size="xs" c="dimmed">
        Note: It only accepts csv file
      </Text>
    </div>
  );
};

export default CSVUpload;
