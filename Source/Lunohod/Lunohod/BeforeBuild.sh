echo removing Metadata in temp
rmdir -r /Metadata
echo copy Metadata to temp
cp -r $1/Metadata /
echo remove whitespaces from metadata
mono $1/XmlWhiteSpaceRemover.exe $1/Metadata/ *.xml $1/Metadata/
